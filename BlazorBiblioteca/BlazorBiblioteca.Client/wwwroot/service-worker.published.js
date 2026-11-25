
// DKDEV Library Nexus - Production Service Worker
// Cachea recursos estáticos y mantiene las páginas y API conectadas al servidor.

const CACHE_PREFIX = "dkdev-library-nexus-";
const CACHE_NAME = `${CACHE_PREFIX}v1`;

self.importScripts("./service-worker-assets.js");

self.addEventListener("install", event => {
    event.waitUntil((async () => {
        const cache = await caches.open(CACHE_NAME);

        const assets = self.assetsManifest.assets
            .filter(asset => !asset.url.includes("service-worker"))
            .map(asset => new Request(
                new URL(asset.url, self.registration.scope),
                {
                    integrity: asset.hash,
                    cache: "reload"
                }
            ));

        await cache.addAll(assets);
        await self.skipWaiting();

        console.info("DKDEV PWA: recursos estáticos almacenados.");
    })());
});

self.addEventListener("activate", event => {
    event.waitUntil((async () => {
        const cacheKeys = await caches.keys();

        await Promise.all(
            cacheKeys
                .filter(key =>
                    key.startsWith(CACHE_PREFIX) &&
                    key !== CACHE_NAME
                )
                .map(key => caches.delete(key))
        );

        await self.clients.claim();

        console.info("DKDEV PWA: Service Worker activo.");
    })());
});

self.addEventListener("fetch", event => {
    const request = event.request;

    // Solo gestionamos solicitudes GET.
    if (request.method !== "GET") {
        return;
    }

    const url = new URL(request.url);

    // No interceptar solicitudes externas.
    if (url.origin !== self.location.origin) {
        return;
    }

    // Las páginas dinámicas deben consultarse siempre al servidor.
    if (request.mode === "navigate") {
        return;
    }

    // La API nunca debe responder desde caché.
    if (url.pathname.startsWith("/api/")) {
        return;
    }

    // No almacenar conexiones internas de Blazor.
    if (
        url.pathname.startsWith("/_blazor") ||
        url.pathname.startsWith("/_framework/aspnetcore-browser-refresh")
    ) {
        return;
    }

    // Recursos estáticos: caché primero y red como alternativa.
    event.respondWith((async () => {
        const cache = await caches.open(CACHE_NAME);
        const cachedResponse = await cache.match(request);

        if (cachedResponse) {
            return cachedResponse;
        }

        try {
            const networkResponse = await fetch(request);

            if (networkResponse.ok && networkResponse.type === "basic") {
                await cache.put(request, networkResponse.clone());
            }

            return networkResponse;
        } catch (error) {
            console.error("DKDEV PWA: recurso no disponible.", error);
            throw error;
        }
    })());
});
