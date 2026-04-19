self.importScripts('./service-worker-assets.js');
self.addEventListener('install',  e => e.waitUntil(onInstall(e)));
self.addEventListener('activate', e => e.waitUntil(onActivate(e)));
self.addEventListener('fetch',    e => e.respondWith(onFetch(e)));

const cachePrefix  = 'offline-cache-';
const cacheName    = `${cachePrefix}${self.assetsManifest.version}`;
const includeRegex = [/\.blat$/,/\.dll$/,/\.wasm$/,/\.html$/,/\.js$/,/\.json$/,/\.css$/,/\.woff2?$/,/\.png$/,/\.jpe?g$/,/\.ico$/,/\.svg$/];
const excludeRegex = [/^service-worker\.js$/];

async function onInstall() {
    self.skipWaiting();
    const requests = self.assetsManifest.assets
        .filter(a => includeRegex.some(r => r.test(a.url)))
        .filter(a => !excludeRegex.some(r => r.test(a.url)))
        .map(a => new Request(a.url, { integrity: a.hash, cache: 'no-cache' }));
    await caches.open(cacheName).then(c => c.addAll(requests));
}

async function onActivate() {
    const keys = await caches.keys();
    await Promise.all(keys
        .filter(k => k.startsWith(cachePrefix) && k !== cacheName)
        .map(k => caches.delete(k)));
}

async function onFetch(event) {
    if (event.request.method !== 'GET') return fetch(event.request);
    const request = event.request.mode === 'navigate' ? 'index.html' : event.request;
    return (await caches.match(request)) || fetch(event.request);
}
