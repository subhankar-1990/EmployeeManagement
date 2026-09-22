import { EnvironmentProviders, makeEnvironmentProviders } from '@angular/core';
import { environment } from '../../../environments/environment';
import { EmployeesClient } from './api-client';

/**
 * Registers `EmployeesClient`, the NSwag generated API client, with the injector.
 *
 * The client defaults to `window` as its HTTP implementation, which does not exist while the
 * application renders on the server, so an explicit implementation is supplied here. `globalThis`
 * exposes `fetch` in both the browser and Node.js, and calling it through `globalThis` keeps the
 * required `this` binding intact.
 */
export function provideEmployeesClient(): EnvironmentProviders {
  return makeEnvironmentProviders([
    {
      provide: EmployeesClient,
      useFactory: (): EmployeesClient =>
        new EmployeesClient(environment.apiBaseUrl, {
          fetch: (url: RequestInfo, init?: RequestInit): Promise<Response> => globalThis.fetch(url, init),
        }),
    },
  ]);
}
