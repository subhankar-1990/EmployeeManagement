
export default {
  bootstrap: () => import('./main.server.mjs').then(m => m.default),
  inlineCriticalCss: false,
  baseHref: '/',
  locale: undefined,
  routes: [
  {
    "renderMode": 1,
    "redirectTo": "/employees",
    "route": "/"
  },
  {
    "renderMode": 1,
    "preload": [
      "chunk-MWYQ2XMG.js",
      "chunk-UCJWJEHN.js",
      "chunk-554XGCVP.js",
      "chunk-LDZDT6CZ.js"
    ],
    "route": "/employees"
  },
  {
    "renderMode": 1,
    "preload": [
      "chunk-MWYQ2XMG.js",
      "chunk-2NUO3JRT.js",
      "chunk-LDZDT6CZ.js"
    ],
    "route": "/employees/new"
  },
  {
    "renderMode": 1,
    "preload": [
      "chunk-MWYQ2XMG.js",
      "chunk-NKLZKAD7.js",
      "chunk-554XGCVP.js",
      "chunk-LDZDT6CZ.js"
    ],
    "route": "/employees/*"
  },
  {
    "renderMode": 1,
    "preload": [
      "chunk-MWYQ2XMG.js",
      "chunk-2NUO3JRT.js",
      "chunk-LDZDT6CZ.js"
    ],
    "route": "/employees/*/edit"
  },
  {
    "renderMode": 1,
    "redirectTo": "/employees",
    "route": "/**"
  }
],
  entryPointToBrowserMapping: undefined,
  assets: {
    'index.csr.html': {size: 500, hash: '22cadc7bb6bf0debd171bf82f0a9ca79ffd952344a99b2cb39861be71f6094ca', text: () => import('./assets-chunks/index_csr_html.mjs').then(m => m.default)},
    'index.server.html': {size: 1040, hash: 'd8bb3c01e8cada6a98267951ef3af1758c34b4a90ee59c94687b7f4546c61122', text: () => import('./assets-chunks/index_server_html.mjs').then(m => m.default)}
  },
};
