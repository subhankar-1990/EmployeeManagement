/**
 * Configuration applied to development builds (`ng serve`, `ng test`).
 *
 * The empty base URL keeps every request same-origin so that the Angular dev server can forward
 * `/api/**` to the ASP.NET Core API, see `src/proxy.conf.json`. Proxying avoids the CORS
 * pre-flight round trip and the browser warning about the API's self-signed development
 * certificate. Point `apiBaseUrl` at `https://localhost:7031/` if you prefer to call the API
 * directly and have configured CORS on the API.
 */
export const environment = {
  production: false,
  apiBaseUrl: '',
};
