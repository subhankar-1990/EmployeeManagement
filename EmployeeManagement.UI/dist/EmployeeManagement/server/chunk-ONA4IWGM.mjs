import './polyfills.server.mjs';
import {
  ConfirmDialog
} from "./chunk-6EAOL6XC.mjs";
import {
  EmployeeService,
  errorMessageOf,
  takeUntilDestroyed
} from "./chunk-WPQ6TRVZ.mjs";
import {
  ActivatedRoute,
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  Router,
  RouterLink,
  computed,
  inject,
  setClassMetadata,
  signal,
  ɵsetClassDebugInfo,
  ɵɵadvance,
  ɵɵclassProp,
  ɵɵconditional,
  ɵɵconditionalCreate,
  ɵɵdefineComponent,
  ɵɵelement,
  ɵɵelementEnd,
  ɵɵelementStart,
  ɵɵgetCurrentView,
  ɵɵlistener,
  ɵɵnextContext,
  ɵɵproperty,
  ɵɵpureFunction1,
  ɵɵresetView,
  ɵɵrestoreView,
  ɵɵsanitizeUrl,
  ɵɵtext,
  ɵɵtextInterpolate,
  ɵɵtextInterpolate1
} from "./chunk-HAJVBCAU.mjs";
import "./chunk-LEH5XX4U.mjs";

// src/app/features/employees/employee-detail/employee-detail.ts
var _c0 = (a0) => ["/employees", a0, "edit"];
function EmployeeDetail_Conditional_8_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275text(0);
  }
  if (rf & 2) {
    \u0275\u0275textInterpolate1(" Record #", ctx, " ");
  }
}
function EmployeeDetail_Conditional_12_Template(rf, ctx) {
  if (rf & 1) {
    const _r1 = \u0275\u0275getCurrentView();
    \u0275\u0275elementStart(0, "a", 10);
    \u0275\u0275text(1, "Edit");
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(2, "button", 11);
    \u0275\u0275listener("click", function EmployeeDetail_Conditional_12_Template_button_click_2_listener() {
      \u0275\u0275restoreView(_r1);
      const ctx_r1 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r1.requestDelete());
    });
    \u0275\u0275text(3, "Delete");
    \u0275\u0275elementEnd();
  }
  if (rf & 2) {
    \u0275\u0275property("routerLink", \u0275\u0275pureFunction1(1, _c0, ctx.id));
  }
}
function EmployeeDetail_Conditional_13_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275elementStart(0, "div", 7)(1, "p", 12);
    \u0275\u0275element(2, "span", 13);
    \u0275\u0275text(3, " Loading employee\u2026 ");
    \u0275\u0275elementEnd()();
  }
}
function EmployeeDetail_Conditional_14_Conditional_4_Template(rf, ctx) {
  if (rf & 1) {
    const _r3 = \u0275\u0275getCurrentView();
    \u0275\u0275elementStart(0, "button", 18);
    \u0275\u0275listener("click", function EmployeeDetail_Conditional_14_Conditional_4_Template_button_click_0_listener() {
      \u0275\u0275restoreView(_r3);
      const ctx_r1 = \u0275\u0275nextContext(2);
      return \u0275\u0275resetView(ctx_r1.reload());
    });
    \u0275\u0275text(1, "Try again");
    \u0275\u0275elementEnd();
  }
}
function EmployeeDetail_Conditional_14_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275elementStart(0, "div", 8)(1, "p", 14);
    \u0275\u0275text(2);
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(3, "div", 15);
    \u0275\u0275conditionalCreate(4, EmployeeDetail_Conditional_14_Conditional_4_Template, 2, 0, "button", 16);
    \u0275\u0275elementStart(5, "a", 17);
    \u0275\u0275text(6, "Back to list");
    \u0275\u0275elementEnd()()();
  }
  if (rf & 2) {
    const ctx_r1 = \u0275\u0275nextContext();
    \u0275\u0275advance(2);
    \u0275\u0275textInterpolate(ctx);
    \u0275\u0275advance(2);
    \u0275\u0275conditional(ctx_r1.employeeId() ? 4 : -1);
  }
}
function EmployeeDetail_Conditional_15_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275elementStart(0, "div", 7)(1, "dl", 19)(2, "div", 20)(3, "dt");
    \u0275\u0275text(4, "Id");
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(5, "dd");
    \u0275\u0275text(6);
    \u0275\u0275elementEnd()();
    \u0275\u0275elementStart(7, "div", 20)(8, "dt");
    \u0275\u0275text(9, "Name");
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(10, "dd");
    \u0275\u0275text(11);
    \u0275\u0275elementEnd()();
    \u0275\u0275elementStart(12, "div", 20)(13, "dt");
    \u0275\u0275text(14, "Mobile");
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(15, "dd")(16, "a", 21);
    \u0275\u0275text(17);
    \u0275\u0275elementEnd()()();
    \u0275\u0275elementStart(18, "div", 20)(19, "dt");
    \u0275\u0275text(20, "Email");
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(21, "dd")(22, "a", 21);
    \u0275\u0275text(23);
    \u0275\u0275elementEnd()()();
    \u0275\u0275elementStart(24, "div", 20)(25, "dt");
    \u0275\u0275text(26, "Status");
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(27, "dd")(28, "span", 22);
    \u0275\u0275text(29);
    \u0275\u0275elementEnd()()()()();
  }
  if (rf & 2) {
    const current_r4 = ctx;
    \u0275\u0275advance(6);
    \u0275\u0275textInterpolate(current_r4.id);
    \u0275\u0275advance(5);
    \u0275\u0275textInterpolate(current_r4.name);
    \u0275\u0275advance(5);
    \u0275\u0275property("href", "tel:" + current_r4.mobile, \u0275\u0275sanitizeUrl);
    \u0275\u0275advance();
    \u0275\u0275textInterpolate(current_r4.mobile);
    \u0275\u0275advance(5);
    \u0275\u0275property("href", "mailto:" + current_r4.email, \u0275\u0275sanitizeUrl);
    \u0275\u0275advance();
    \u0275\u0275textInterpolate(current_r4.email);
    \u0275\u0275advance(5);
    \u0275\u0275classProp("badge--active", current_r4.isActive)("badge--inactive", !current_r4.isActive);
    \u0275\u0275advance();
    \u0275\u0275textInterpolate1(" ", current_r4.isActive ? "Active" : "Inactive", " ");
  }
}
function EmployeeDetail_Conditional_16_Template(rf, ctx) {
  if (rf & 1) {
    const _r5 = \u0275\u0275getCurrentView();
    \u0275\u0275elementStart(0, "app-confirm-dialog", 23);
    \u0275\u0275listener("confirmed", function EmployeeDetail_Conditional_16_Template_app_confirm_dialog_confirmed_0_listener() {
      \u0275\u0275restoreView(_r5);
      const ctx_r1 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r1.confirmDelete());
    })("cancelled", function EmployeeDetail_Conditional_16_Template_app_confirm_dialog_cancelled_0_listener() {
      \u0275\u0275restoreView(_r5);
      const ctx_r1 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r1.cancelDelete());
    });
    \u0275\u0275elementEnd();
  }
  if (rf & 2) {
    const ctx_r1 = \u0275\u0275nextContext();
    \u0275\u0275property("message", ctx_r1.deleteMessage())("busy", ctx_r1.deleteBusy());
  }
}
var EmployeeDetail = class _EmployeeDetail {
  employeeService = inject(EmployeeService);
  route = inject(ActivatedRoute);
  router = inject(Router);
  destroyRef = inject(DestroyRef);
  employee = signal(
    null,
    ...ngDevMode ? [{ debugName: "employee" }] : (
      /* istanbul ignore next */
      []
    )
  );
  employeeId = signal(
    null,
    ...ngDevMode ? [{ debugName: "employeeId" }] : (
      /* istanbul ignore next */
      []
    )
  );
  loading = signal(
    false,
    ...ngDevMode ? [{ debugName: "loading" }] : (
      /* istanbul ignore next */
      []
    )
  );
  errorMessage = signal(
    null,
    ...ngDevMode ? [{ debugName: "errorMessage" }] : (
      /* istanbul ignore next */
      []
    )
  );
  deleteRequested = signal(
    false,
    ...ngDevMode ? [{ debugName: "deleteRequested" }] : (
      /* istanbul ignore next */
      []
    )
  );
  deleteBusy = signal(
    false,
    ...ngDevMode ? [{ debugName: "deleteBusy" }] : (
      /* istanbul ignore next */
      []
    )
  );
  /** Prompt shown by the delete confirmation dialog. */
  deleteMessage = computed(
    () => {
      const employee = this.employee();
      return `Delete "${employee?.name ?? "this employee"}" permanently? This cannot be undone.`;
    },
    ...ngDevMode ? [{ debugName: "deleteMessage" }] : (
      /* istanbul ignore next */
      []
    )
  );
  constructor() {
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      const id = Number(params.get("id"));
      if (!Number.isInteger(id) || id <= 0) {
        this.employeeId.set(null);
        this.employee.set(null);
        this.errorMessage.set("The employee id in the address is not valid.");
        return;
      }
      this.employeeId.set(id);
      this.load(id);
    });
  }
  /** Re-requests the employee currently displayed. */
  reload() {
    const id = this.employeeId();
    if (id !== null) {
      this.load(id);
    }
  }
  requestDelete() {
    this.deleteRequested.set(true);
  }
  cancelDelete() {
    if (!this.deleteBusy()) {
      this.deleteRequested.set(false);
    }
  }
  confirmDelete() {
    const id = this.employeeId();
    if (id === null) {
      return;
    }
    this.deleteBusy.set(true);
    this.employeeService.deleteEmployee(id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.deleteBusy.set(false);
        this.deleteRequested.set(false);
        void this.router.navigate(["/employees"], { queryParams: { deleted: 1 } });
      },
      error: (error) => {
        this.deleteBusy.set(false);
        this.deleteRequested.set(false);
        this.errorMessage.set(errorMessageOf(error));
      }
    });
  }
  load(id) {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.employeeService.getEmployeeById(id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (employee) => {
        this.employee.set(employee);
        this.loading.set(false);
      },
      error: (error) => {
        this.employee.set(null);
        this.errorMessage.set(errorMessageOf(error));
        this.loading.set(false);
      }
    });
  }
  static \u0275fac = function EmployeeDetail_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _EmployeeDetail)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _EmployeeDetail, selectors: [["app-employee-detail"]], decls: 17, vars: 5, consts: [[1, "page"], [1, "page__header"], [1, "page__eyebrow"], [1, "page__title"], [1, "page__subtitle"], [1, "page__actions"], ["routerLink", "/employees", 1, "btn", "btn--ghost"], [1, "card"], ["role", "alert", 1, "alert", "alert--error"], ["title", "Delete employee", "confirmLabel", "Delete", 3, "message", "busy"], [1, "btn", "btn--secondary", 3, "routerLink"], ["type", "button", 1, "btn", "btn--danger", 3, "click"], ["role", "status", 1, "status-line"], ["aria-hidden", "true", 1, "spinner", "spinner--sm"], [1, "alert__message"], [1, "alert__actions"], ["type", "button", 1, "btn", "btn--secondary", "btn--sm"], ["routerLink", "/employees", 1, "btn", "btn--ghost", "btn--sm"], ["type", "button", 1, "btn", "btn--secondary", "btn--sm", 3, "click"], [1, "details"], [1, "details__row"], [3, "href"], [1, "badge"], ["title", "Delete employee", "confirmLabel", "Delete", 3, "confirmed", "cancelled", "message", "busy"]], template: function EmployeeDetail_Template(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275elementStart(0, "section", 0)(1, "header", 1)(2, "div")(3, "p", 2);
      \u0275\u0275text(4, "Employee details");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(5, "h1", 3);
      \u0275\u0275text(6);
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(7, "p", 4);
      \u0275\u0275conditionalCreate(8, EmployeeDetail_Conditional_8_Template, 1, 1);
      \u0275\u0275elementEnd()();
      \u0275\u0275elementStart(9, "div", 5)(10, "a", 6);
      \u0275\u0275text(11, "Back to list");
      \u0275\u0275elementEnd();
      \u0275\u0275conditionalCreate(12, EmployeeDetail_Conditional_12_Template, 4, 3);
      \u0275\u0275elementEnd()();
      \u0275\u0275conditionalCreate(13, EmployeeDetail_Conditional_13_Template, 4, 0, "div", 7)(14, EmployeeDetail_Conditional_14_Template, 7, 2, "div", 8)(15, EmployeeDetail_Conditional_15_Template, 30, 11, "div", 7);
      \u0275\u0275elementEnd();
      \u0275\u0275conditionalCreate(16, EmployeeDetail_Conditional_16_Template, 1, 2, "app-confirm-dialog", 9);
    }
    if (rf & 2) {
      let tmp_1_0;
      let tmp_2_0;
      let tmp_3_0;
      \u0275\u0275advance(6);
      \u0275\u0275textInterpolate(ctx.employee()?.name ?? "Employee");
      \u0275\u0275advance(2);
      \u0275\u0275conditional((tmp_1_0 = ctx.employeeId()) ? 8 : -1, tmp_1_0);
      \u0275\u0275advance(4);
      \u0275\u0275conditional((tmp_2_0 = ctx.employee()) ? 12 : -1, tmp_2_0);
      \u0275\u0275advance();
      \u0275\u0275conditional(ctx.loading() ? 13 : (tmp_3_0 = ctx.errorMessage()) ? 14 : (tmp_3_0 = ctx.employee()) ? 15 : -1, tmp_3_0);
      \u0275\u0275advance(3);
      \u0275\u0275conditional(ctx.deleteRequested() ? 16 : -1);
    }
  }, dependencies: [RouterLink, ConfirmDialog], styles: ["\n.details[_ngcontent-%COMP%] {\n  display: grid;\n  gap: 0;\n  margin: 0;\n}\n.details__row[_ngcontent-%COMP%] {\n  display: grid;\n  grid-template-columns: minmax(7rem, 12rem) 1fr;\n  gap: 1rem;\n  padding: 0.85rem 0;\n  border-bottom: 1px solid var(--%NS%color-border);\n}\n.details__row[_ngcontent-%COMP%]:last-child {\n  border-bottom: 0;\n  padding-bottom: 0;\n}\n.details__row[_ngcontent-%COMP%]:first-child {\n  padding-top: 0;\n}\n.details[_ngcontent-%COMP%]   dt[_ngcontent-%COMP%] {\n  color: var(--%NS%color-muted);\n  font-size: 0.875rem;\n  font-weight: 500;\n}\n.details[_ngcontent-%COMP%]   dd[_ngcontent-%COMP%] {\n  margin: 0;\n  word-break: break-word;\n}\n.details[_ngcontent-%COMP%]   dd[_ngcontent-%COMP%]   a[_ngcontent-%COMP%] {\n  color: var(--%NS%color-primary);\n}\n/*# sourceMappingURL=employee-detail.css.map */"] });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(EmployeeDetail, [{
    type: Component,
    args: [{ selector: "app-employee-detail", imports: [RouterLink, ConfirmDialog], changeDetection: ChangeDetectionStrategy.OnPush, template: `<section class="page">\r
  <header class="page__header">\r
    <div>\r
      <p class="page__eyebrow">Employee details</p>\r
      <h1 class="page__title">{{ employee()?.name ?? 'Employee' }}</h1>\r
      <p class="page__subtitle">\r
        @if (employeeId(); as id) {\r
          Record #{{ id }}\r
        }\r
      </p>\r
    </div>\r
\r
    <div class="page__actions">\r
      <a class="btn btn--ghost" routerLink="/employees">Back to list</a>\r
      @if (employee(); as current) {\r
        <a class="btn btn--secondary" [routerLink]="['/employees', current.id, 'edit']">Edit</a>\r
        <button type="button" class="btn btn--danger" (click)="requestDelete()">Delete</button>\r
      }\r
    </div>\r
  </header>\r
\r
  @if (loading()) {\r
    <div class="card">\r
      <p class="status-line" role="status">\r
        <span class="spinner spinner--sm" aria-hidden="true"></span>\r
        Loading employee&hellip;\r
      </p>\r
    </div>\r
  } @else if (errorMessage(); as message) {\r
    <div class="alert alert--error" role="alert">\r
      <p class="alert__message">{{ message }}</p>\r
      <div class="alert__actions">\r
        @if (employeeId()) {\r
          <button type="button" class="btn btn--secondary btn--sm" (click)="reload()">Try again</button>\r
        }\r
        <a class="btn btn--ghost btn--sm" routerLink="/employees">Back to list</a>\r
      </div>\r
    </div>\r
  } @else if (employee(); as current) {\r
    <div class="card">\r
      <dl class="details">\r
        <div class="details__row">\r
          <dt>Id</dt>\r
          <dd>{{ current.id }}</dd>\r
        </div>\r
        <div class="details__row">\r
          <dt>Name</dt>\r
          <dd>{{ current.name }}</dd>\r
        </div>\r
        <div class="details__row">\r
          <dt>Mobile</dt>\r
          <dd><a [href]="'tel:' + current.mobile">{{ current.mobile }}</a></dd>\r
        </div>\r
        <div class="details__row">\r
          <dt>Email</dt>\r
          <dd><a [href]="'mailto:' + current.email">{{ current.email }}</a></dd>\r
        </div>\r
        <div class="details__row">\r
          <dt>Status</dt>\r
          <dd>\r
            <span class="badge" [class.badge--active]="current.isActive" [class.badge--inactive]="!current.isActive">\r
              {{ current.isActive ? 'Active' : 'Inactive' }}\r
            </span>\r
          </dd>\r
        </div>\r
      </dl>\r
    </div>\r
  }\r
</section>\r
\r
@if (deleteRequested()) {\r
  <app-confirm-dialog\r
    title="Delete employee"\r
    [message]="deleteMessage()"\r
    confirmLabel="Delete"\r
    [busy]="deleteBusy()"\r
    (confirmed)="confirmDelete()"\r
    (cancelled)="cancelDelete()"\r
  />\r
}\r
`, styles: ["/* src/app/features/employees/employee-detail/employee-detail.scss */\n.details {\n  display: grid;\n  gap: 0;\n  margin: 0;\n}\n.details__row {\n  display: grid;\n  grid-template-columns: minmax(7rem, 12rem) 1fr;\n  gap: 1rem;\n  padding: 0.85rem 0;\n  border-bottom: 1px solid var(--color-border);\n}\n.details__row:last-child {\n  border-bottom: 0;\n  padding-bottom: 0;\n}\n.details__row:first-child {\n  padding-top: 0;\n}\n.details dt {\n  color: var(--color-muted);\n  font-size: 0.875rem;\n  font-weight: 500;\n}\n.details dd {\n  margin: 0;\n  word-break: break-word;\n}\n.details dd a {\n  color: var(--color-primary);\n}\n/*# sourceMappingURL=employee-detail.css.map */\n"] }]
  }], () => [], null);
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(EmployeeDetail, { className: "EmployeeDetail", filePath: "src/app/features/employees/employee-detail/employee-detail.ts", lineNumber: 20 });
})();
export {
  EmployeeDetail
};
//# sourceMappingURL=chunk-ONA4IWGM.mjs.map
