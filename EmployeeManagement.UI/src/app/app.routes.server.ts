import { RenderMode, ServerRoute } from '@angular/ssr';

/**
 * Every route renders in the browser.
 *
 * The screens read live employee data from the API, and pre-rendering them at build time would mean
 * the build has to reach the API and would bake one snapshot of the data into the static output.
 * Client rendering keeps the payload free of stale data; the shell is static anyway.
 */
export const serverRoutes: ServerRoute[] = [
  {
    path: '**',
    renderMode: RenderMode.Client,
  },
];
