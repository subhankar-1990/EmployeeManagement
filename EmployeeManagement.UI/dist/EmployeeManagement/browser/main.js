import {
  ChangeDetectionStrategy,
  Component,
  EmployeesClient,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
  bootstrapApplication,
  makeEnvironmentProviders,
  provideBrowserGlobalErrorListeners,
  provideClientHydration,
  provideRouter,
  setClassMetadata,
  signal,
  withInMemoryScrolling,
  ɵsetClassDebugInfo,
  ɵɵadvance,
  ɵɵdefineComponent,
  ɵɵelement,
  ɵɵelementEnd,
  ɵɵelementStart,
  ɵɵproperty,
  ɵɵpureFunction0,
  ɵɵtext,
  ɵɵtextInterpolate
} from "./chunk-3KAAG4ND.js";
import {
  __spreadValues
} from "./chunk-GOMI4DH3.js";

// src/environments/environment.ts
var environment = {
  production: false,
  apiBaseUrl: ""
};

// src/app/core/services/api-client.providers.ts
function provideEmployeesClient() {
  return makeEnvironmentProviders([
    {
      provide: EmployeesClient,
      useFactory: () => new EmployeesClient(environment.apiBaseUrl, {
        fetch: (url, init) => globalThis.fetch(url, init)
      })
    }
  ]);
}

// src/app/app.routes.ts
var routes = [
  { path: "", pathMatch: "full", redirectTo: "employees" },
  __spreadValues({
    path: "employees",
    loadChildren: () => import("./chunk-MWYQ2XMG.js").then((module) => module.EMPLOYEES_ROUTES)
  }, false ? { \u0275entryName: "src/app/features/employees/employees.routes.ts" } : {}),
  { path: "**", redirectTo: "employees" }
];

// src/app/app.config.ts
var appConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withInMemoryScrolling({ scrollPositionRestoration: "enabled" })),
    provideClientHydration(),
    provideEmployeesClient()
  ]
};

// src/app/app.ts
var _c0 = () => ({ exact: false });
var App = class _App {
  title = signal(
    "Employee Management",
    ...ngDevMode ? [{ debugName: "title" }] : (
      /* istanbul ignore next */
      []
    )
  );
  static \u0275fac = function App_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _App)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _App, selectors: [["app-root"]], decls: 16, vars: 3, consts: [[1, "shell"], [1, "shell__header"], ["routerLink", "/employees", 1, "brand"], ["aria-hidden", "true", 1, "brand__mark"], [1, "brand__text"], ["aria-label", "Main navigation", 1, "shell__nav"], ["routerLink", "/employees", "routerLinkActive", "is-active", 1, "shell__link", 3, "routerLinkActiveOptions"], ["routerLink", "/employees/new", "routerLinkActive", "is-active", 1, "shell__link"], [1, "shell__main"], [1, "shell__footer"]], template: function App_Template(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275elementStart(0, "div", 0)(1, "header", 1)(2, "a", 2)(3, "span", 3);
      \u0275\u0275text(4, "EM");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(5, "span", 4);
      \u0275\u0275text(6);
      \u0275\u0275elementEnd()();
      \u0275\u0275elementStart(7, "nav", 5)(8, "a", 6);
      \u0275\u0275text(9, " Employees ");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(10, "a", 7);
      \u0275\u0275text(11, "Add employee");
      \u0275\u0275elementEnd()()();
      \u0275\u0275elementStart(12, "main", 8);
      \u0275\u0275element(13, "router-outlet");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(14, "footer", 9);
      \u0275\u0275text(15, "Employee Management \xB7 ASP.NET Core Web API");
      \u0275\u0275elementEnd()();
    }
    if (rf & 2) {
      \u0275\u0275advance(6);
      \u0275\u0275textInterpolate(ctx.title());
      \u0275\u0275advance(2);
      \u0275\u0275property("routerLinkActiveOptions", \u0275\u0275pureFunction0(2, _c0));
    }
  }, dependencies: [RouterOutlet, RouterLink, RouterLinkActive], styles: ["\n.shell[_ngcontent-%COMP%] {\n  display: flex;\n  flex-direction: column;\n  min-height: 100vh;\n}\n.shell__header[_ngcontent-%COMP%] {\n  display: flex;\n  flex-wrap: wrap;\n  align-items: center;\n  justify-content: space-between;\n  gap: 1rem;\n  padding: 1rem 1.75rem;\n  background: var(--%NS%color-surface);\n  border-bottom: 1px solid var(--%NS%color-border);\n  box-shadow: var(--%NS%shadow-sm);\n}\n.brand[_ngcontent-%COMP%] {\n  display: inline-flex;\n  align-items: center;\n  gap: 0.65rem;\n  color: inherit;\n  text-decoration: none;\n}\n.brand__mark[_ngcontent-%COMP%] {\n  display: inline-flex;\n  align-items: center;\n  justify-content: center;\n  width: 2.25rem;\n  height: 2.25rem;\n  border-radius: 0.65rem;\n  background: var(--%NS%color-primary);\n  color: #fff;\n  font-size: 0.875rem;\n  font-weight: 700;\n}\n.brand__text[_ngcontent-%COMP%] {\n  font-size: 1.0625rem;\n  font-weight: 600;\n}\n.shell__nav[_ngcontent-%COMP%] {\n  display: flex;\n  gap: 0.5rem;\n}\n.shell__link[_ngcontent-%COMP%] {\n  padding: 0.45rem 0.85rem;\n  border-radius: var(--%NS%radius-md);\n  color: var(--%NS%color-muted);\n  font-size: 0.9375rem;\n  text-decoration: none;\n}\n.shell__link[_ngcontent-%COMP%]:hover {\n  background: var(--%NS%color-bg);\n  color: var(--%NS%color-text);\n}\n.shell__link.is-active[_ngcontent-%COMP%] {\n  background: var(--%NS%color-primary-soft);\n  color: var(--%NS%color-primary-dark);\n  font-weight: 600;\n}\n.shell__main[_ngcontent-%COMP%] {\n  flex: 1;\n  width: 100%;\n  max-width: 72rem;\n  margin: 0 auto;\n  padding: 1.75rem;\n}\n.shell__footer[_ngcontent-%COMP%] {\n  padding: 1rem 1.75rem;\n  border-top: 1px solid var(--%NS%color-border);\n  color: var(--%NS%color-muted);\n  font-size: 0.8125rem;\n  text-align: center;\n}\n@media (max-width: 40rem) {\n  .shell__header[_ngcontent-%COMP%], \n   .shell__main[_ngcontent-%COMP%] {\n    padding-inline: 1rem;\n  }\n}\n/*# sourceMappingURL=app.css.map */"] });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(App, [{
    type: Component,
    args: [{ selector: "app-root", imports: [RouterOutlet, RouterLink, RouterLinkActive], changeDetection: ChangeDetectionStrategy.OnPush, template: '<div class="shell">\r\n  <header class="shell__header">\r\n    <a class="brand" routerLink="/employees">\r\n      <span class="brand__mark" aria-hidden="true">EM</span>\r\n      <span class="brand__text">{{ title() }}</span>\r\n    </a>\r\n\r\n    <nav class="shell__nav" aria-label="Main navigation">\r\n      <a\r\n        class="shell__link"\r\n        routerLink="/employees"\r\n        routerLinkActive="is-active"\r\n        [routerLinkActiveOptions]="{ exact: false }"\r\n      >\r\n        Employees\r\n      </a>\r\n      <a class="shell__link" routerLink="/employees/new" routerLinkActive="is-active">Add employee</a>\r\n    </nav>\r\n  </header>\r\n\r\n  <main class="shell__main">\r\n    <router-outlet />\r\n  </main>\r\n\r\n  <footer class="shell__footer">Employee Management &middot; ASP.NET Core Web API</footer>\r\n</div>\r\n', styles: ["/* src/app/app.scss */\n.shell {\n  display: flex;\n  flex-direction: column;\n  min-height: 100vh;\n}\n.shell__header {\n  display: flex;\n  flex-wrap: wrap;\n  align-items: center;\n  justify-content: space-between;\n  gap: 1rem;\n  padding: 1rem 1.75rem;\n  background: var(--color-surface);\n  border-bottom: 1px solid var(--color-border);\n  box-shadow: var(--shadow-sm);\n}\n.brand {\n  display: inline-flex;\n  align-items: center;\n  gap: 0.65rem;\n  color: inherit;\n  text-decoration: none;\n}\n.brand__mark {\n  display: inline-flex;\n  align-items: center;\n  justify-content: center;\n  width: 2.25rem;\n  height: 2.25rem;\n  border-radius: 0.65rem;\n  background: var(--color-primary);\n  color: #fff;\n  font-size: 0.875rem;\n  font-weight: 700;\n}\n.brand__text {\n  font-size: 1.0625rem;\n  font-weight: 600;\n}\n.shell__nav {\n  display: flex;\n  gap: 0.5rem;\n}\n.shell__link {\n  padding: 0.45rem 0.85rem;\n  border-radius: var(--radius-md);\n  color: var(--color-muted);\n  font-size: 0.9375rem;\n  text-decoration: none;\n}\n.shell__link:hover {\n  background: var(--color-bg);\n  color: var(--color-text);\n}\n.shell__link.is-active {\n  background: var(--color-primary-soft);\n  color: var(--color-primary-dark);\n  font-weight: 600;\n}\n.shell__main {\n  flex: 1;\n  width: 100%;\n  max-width: 72rem;\n  margin: 0 auto;\n  padding: 1.75rem;\n}\n.shell__footer {\n  padding: 1rem 1.75rem;\n  border-top: 1px solid var(--color-border);\n  color: var(--color-muted);\n  font-size: 0.8125rem;\n  text-align: center;\n}\n@media (max-width: 40rem) {\n  .shell__header,\n  .shell__main {\n    padding-inline: 1rem;\n  }\n}\n/*# sourceMappingURL=app.css.map */\n"] }]
  }], null, null);
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(App, { className: "App", filePath: "src/app/app.ts", lineNumber: 11 });
})();

// src/main.ts
bootstrapApplication(App, appConfig).catch((err) => console.error(err));
//# debugId=c901c792-47e1-58c9-8e37-bd07e3b01b4f
//# sourceMappingURL=main.js.map
