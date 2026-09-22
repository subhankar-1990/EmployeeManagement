import {
  ConfirmDialog
} from "./chunk-554XGCVP.js";
import {
  DEFAULT_PAGE_NUMBER,
  DEFAULT_PAGE_SIZE,
  EmployeeService,
  PAGE_SIZE_OPTIONS,
  SEARCH_DEBOUNCE_MS,
  errorMessageOf,
  takeUntilDestroyed
} from "./chunk-LDZDT6CZ.js";
import {
  ActivatedRoute,
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  EMPTY,
  Input,
  Output,
  Router,
  RouterLink,
  Subject,
  catchError,
  computed,
  debounceTime,
  distinctUntilChanged,
  inject,
  input,
  output,
  setClassMetadata,
  signal,
  switchMap,
  tap,
  ɵsetClassDebugInfo,
  ɵɵadvance,
  ɵɵattribute,
  ɵɵclassProp,
  ɵɵconditional,
  ɵɵconditionalCreate,
  ɵɵdefineComponent,
  ɵɵdomElementEnd,
  ɵɵdomElementStart,
  ɵɵdomListener,
  ɵɵdomProperty,
  ɵɵelement,
  ɵɵelementEnd,
  ɵɵelementStart,
  ɵɵgetCurrentView,
  ɵɵlistener,
  ɵɵnextContext,
  ɵɵproperty,
  ɵɵpureFunction1,
  ɵɵrepeater,
  ɵɵrepeaterCreate,
  ɵɵrepeaterTrackByIdentity,
  ɵɵresetView,
  ɵɵrestoreView,
  ɵɵtext,
  ɵɵtextInterpolate,
  ɵɵtextInterpolate1
} from "./chunk-3KAAG4ND.js";
import "./chunk-GOMI4DH3.js";

// src/app/shared/components/pagination/pagination.ts
function Pagination_Conditional_2_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275text(0, " No results ");
  }
}
function Pagination_Conditional_3_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275text(0, " Showing ");
    \u0275\u0275domElementStart(1, "strong");
    \u0275\u0275text(2);
    \u0275\u0275domElementEnd();
    \u0275\u0275text(3, "\u2013");
    \u0275\u0275domElementStart(4, "strong");
    \u0275\u0275text(5);
    \u0275\u0275domElementEnd();
    \u0275\u0275text(6, " of ");
    \u0275\u0275domElementStart(7, "strong");
    \u0275\u0275text(8);
    \u0275\u0275domElementEnd();
  }
  if (rf & 2) {
    const ctx_r0 = \u0275\u0275nextContext();
    \u0275\u0275advance(2);
    \u0275\u0275textInterpolate(ctx_r0.firstRowNumber());
    \u0275\u0275advance(3);
    \u0275\u0275textInterpolate(ctx_r0.lastRowNumber());
    \u0275\u0275advance(3);
    \u0275\u0275textInterpolate(ctx_r0.totalCount());
  }
}
function Pagination_For_10_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275domElementStart(0, "option", 5);
    \u0275\u0275text(1);
    \u0275\u0275domElementEnd();
  }
  if (rf & 2) {
    const option_r2 = ctx.$implicit;
    \u0275\u0275domProperty("value", option_r2);
    \u0275\u0275advance();
    \u0275\u0275textInterpolate(option_r2);
  }
}
function Pagination_For_15_Template(rf, ctx) {
  if (rf & 1) {
    const _r3 = \u0275\u0275getCurrentView();
    \u0275\u0275domElementStart(0, "button", 9);
    \u0275\u0275domListener("click", function Pagination_For_15_Template_button_click_0_listener() {
      const page_r4 = \u0275\u0275restoreView(_r3).$implicit;
      const ctx_r0 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r0.goToPage(page_r4));
    });
    \u0275\u0275text(1);
    \u0275\u0275domElementEnd();
  }
  if (rf & 2) {
    const page_r4 = ctx.$implicit;
    const ctx_r0 = \u0275\u0275nextContext();
    \u0275\u0275classProp("is-current", page_r4 === ctx_r0.pageNumber());
    \u0275\u0275domProperty("disabled", ctx_r0.disabled());
    \u0275\u0275attribute("aria-current", page_r4 === ctx_r0.pageNumber() ? "page" : null);
    \u0275\u0275advance();
    \u0275\u0275textInterpolate1(" ", page_r4, " ");
  }
}
var Pagination = class _Pagination {
  /** One based page that is currently displayed. */
  pageNumber = input.required(
    ...ngDevMode ? [{ debugName: "pageNumber" }] : (
      /* istanbul ignore next */
      []
    )
  );
  /** Rows per page that is currently applied. */
  pageSize = input.required(
    ...ngDevMode ? [{ debugName: "pageSize" }] : (
      /* istanbul ignore next */
      []
    )
  );
  /** Total number of rows matching the current filter. */
  totalCount = input.required(
    ...ngDevMode ? [{ debugName: "totalCount" }] : (
      /* istanbul ignore next */
      []
    )
  );
  /** Total number of pages reported by the API. */
  totalPages = input.required(
    ...ngDevMode ? [{ debugName: "totalPages" }] : (
      /* istanbul ignore next */
      []
    )
  );
  hasPreviousPage = input.required(
    ...ngDevMode ? [{ debugName: "hasPreviousPage" }] : (
      /* istanbul ignore next */
      []
    )
  );
  hasNextPage = input.required(
    ...ngDevMode ? [{ debugName: "hasNextPage" }] : (
      /* istanbul ignore next */
      []
    )
  );
  /** Disables every control, e.g. while a request is in flight. */
  disabled = input(
    false,
    ...ngDevMode ? [{ debugName: "disabled" }] : (
      /* istanbul ignore next */
      []
    )
  );
  /** Numbers offered by the rows-per-page select. */
  pageSizeOptions = input(
    PAGE_SIZE_OPTIONS,
    ...ngDevMode ? [{ debugName: "pageSizeOptions" }] : (
      /* istanbul ignore next */
      []
    )
  );
  pageChange = output();
  pageSizeChange = output();
  /** 1 based number of the first row on the current page, or 0 when the collection is empty. */
  firstRowNumber = computed(
    () => this.totalCount() === 0 ? 0 : (this.pageNumber() - 1) * this.pageSize() + 1,
    ...ngDevMode ? [{ debugName: "firstRowNumber" }] : (
      /* istanbul ignore next */
      []
    )
  );
  /** 1 based number of the last row on the current page. */
  lastRowNumber = computed(
    () => Math.min(this.pageNumber() * this.pageSize(), this.totalCount()),
    ...ngDevMode ? [{ debugName: "lastRowNumber" }] : (
      /* istanbul ignore next */
      []
    )
  );
  /** Window of page numbers around the current page, five entries at most. */
  pages = computed(
    () => {
      const total = this.totalPages();
      if (total <= 0) {
        return [];
      }
      const windowSize = 5;
      let end = Math.min(total, Math.max(this.pageNumber(), 1) + 2);
      let start = Math.max(1, end - windowSize + 1);
      end = Math.min(total, start + windowSize - 1);
      const result = [];
      for (let page = start; page <= end; page++) {
        result.push(page);
      }
      return result;
    },
    ...ngDevMode ? [{ debugName: "pages" }] : (
      /* istanbul ignore next */
      []
    )
  );
  goToPage(page) {
    if (this.disabled()) {
      return;
    }
    const highest = Math.max(this.totalPages(), 1);
    const target = Math.min(Math.max(page, 1), highest);
    if (target !== this.pageNumber()) {
      this.pageChange.emit(target);
    }
  }
  onPageSizeChange(event) {
    const selected = Number(event.target.value);
    if (!Number.isNaN(selected) && selected !== this.pageSize()) {
      this.pageSizeChange.emit(selected);
    }
  }
  static \u0275fac = function Pagination_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _Pagination)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _Pagination, selectors: [["app-pagination"]], inputs: { pageNumber: [1, "pageNumber"], pageSize: [1, "pageSize"], totalCount: [1, "totalCount"], totalPages: [1, "totalPages"], hasPreviousPage: [1, "hasPreviousPage"], hasNextPage: [1, "hasNextPage"], disabled: [1, "disabled"], pageSizeOptions: [1, "pageSizeOptions"] }, outputs: { pageChange: "pageChange", pageSizeChange: "pageSizeChange" }, decls: 18, vars: 5, consts: [["aria-label", "Pagination", 1, "pagination"], [1, "pagination__summary"], [1, "pagination__controls"], [1, "pagination__size"], [1, "field__control", "field__control--compact", 3, "change", "value", "disabled"], [3, "value"], [1, "pagination__pages"], ["type", "button", 1, "btn", "btn--secondary", "btn--sm", 3, "click", "disabled"], ["type", "button", 1, "btn", "btn--sm", "pagination__page", 3, "is-current", "disabled"], ["type", "button", 1, "btn", "btn--sm", "pagination__page", 3, "click", "disabled"]], template: function Pagination_Template(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275domElementStart(0, "nav", 0)(1, "p", 1);
      \u0275\u0275conditionalCreate(2, Pagination_Conditional_2_Template, 1, 0)(3, Pagination_Conditional_3_Template, 9, 3);
      \u0275\u0275domElementEnd();
      \u0275\u0275domElementStart(4, "div", 2)(5, "label", 3)(6, "span");
      \u0275\u0275text(7, "Rows per page");
      \u0275\u0275domElementEnd();
      \u0275\u0275domElementStart(8, "select", 4);
      \u0275\u0275domListener("change", function Pagination_Template_select_change_8_listener($event) {
        return ctx.onPageSizeChange($event);
      });
      \u0275\u0275repeaterCreate(9, Pagination_For_10_Template, 2, 2, "option", 5, \u0275\u0275repeaterTrackByIdentity);
      \u0275\u0275domElementEnd()();
      \u0275\u0275domElementStart(11, "div", 6)(12, "button", 7);
      \u0275\u0275domListener("click", function Pagination_Template_button_click_12_listener() {
        return ctx.goToPage(ctx.pageNumber() - 1);
      });
      \u0275\u0275text(13, " Previous ");
      \u0275\u0275domElementEnd();
      \u0275\u0275repeaterCreate(14, Pagination_For_15_Template, 2, 5, "button", 8, \u0275\u0275repeaterTrackByIdentity);
      \u0275\u0275domElementStart(16, "button", 7);
      \u0275\u0275domListener("click", function Pagination_Template_button_click_16_listener() {
        return ctx.goToPage(ctx.pageNumber() + 1);
      });
      \u0275\u0275text(17, " Next ");
      \u0275\u0275domElementEnd()()()();
    }
    if (rf & 2) {
      \u0275\u0275advance(2);
      \u0275\u0275conditional(ctx.totalCount() === 0 ? 2 : 3);
      \u0275\u0275advance(6);
      \u0275\u0275domProperty("value", ctx.pageSize())("disabled", ctx.disabled());
      \u0275\u0275advance();
      \u0275\u0275repeater(ctx.pageSizeOptions());
      \u0275\u0275advance(3);
      \u0275\u0275domProperty("disabled", ctx.disabled() || !ctx.hasPreviousPage());
      \u0275\u0275advance(2);
      \u0275\u0275repeater(ctx.pages());
      \u0275\u0275advance(2);
      \u0275\u0275domProperty("disabled", ctx.disabled() || !ctx.hasNextPage());
    }
  }, styles: ["\n.pagination[_ngcontent-%COMP%] {\n  display: flex;\n  flex-wrap: wrap;\n  gap: 1rem;\n  align-items: center;\n  justify-content: space-between;\n  padding: 0.85rem 1.25rem;\n  border-top: 1px solid var(--%NS%color-border);\n}\n.pagination__summary[_ngcontent-%COMP%] {\n  margin: 0;\n  color: var(--%NS%color-muted);\n  font-size: 0.875rem;\n}\n.pagination__controls[_ngcontent-%COMP%] {\n  display: flex;\n  flex-wrap: wrap;\n  align-items: center;\n  gap: 1rem;\n}\n.pagination__size[_ngcontent-%COMP%] {\n  display: flex;\n  align-items: center;\n  gap: 0.5rem;\n  font-size: 0.875rem;\n  color: var(--%NS%color-muted);\n}\n.pagination__pages[_ngcontent-%COMP%] {\n  display: flex;\n  align-items: center;\n  gap: 0.25rem;\n}\n.pagination__page[_ngcontent-%COMP%] {\n  min-width: 2.25rem;\n}\n.pagination__page.is-current[_ngcontent-%COMP%] {\n  background: var(--%NS%color-primary);\n  border-color: var(--%NS%color-primary);\n  color: #fff;\n}\n/*# sourceMappingURL=pagination.css.map */"] });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(Pagination, [{
    type: Component,
    args: [{ selector: "app-pagination", imports: [], changeDetection: ChangeDetectionStrategy.OnPush, template: `<nav class="pagination" aria-label="Pagination">\r
  <p class="pagination__summary">\r
    @if (totalCount() === 0) {\r
      No results\r
    } @else {\r
      Showing <strong>{{ firstRowNumber() }}</strong>\u2013<strong>{{ lastRowNumber() }}</strong> of\r
      <strong>{{ totalCount() }}</strong>\r
    }\r
  </p>\r
\r
  <div class="pagination__controls">\r
    <label class="pagination__size">\r
      <span>Rows per page</span>\r
      <select class="field__control field__control--compact" [value]="pageSize()" [disabled]="disabled()" (change)="onPageSizeChange($event)">\r
        @for (option of pageSizeOptions(); track option) {\r
          <option [value]="option">{{ option }}</option>\r
        }\r
      </select>\r
    </label>\r
\r
    <div class="pagination__pages">\r
      <button\r
        type="button"\r
        class="btn btn--secondary btn--sm"\r
        [disabled]="disabled() || !hasPreviousPage()"\r
        (click)="goToPage(pageNumber() - 1)"\r
      >\r
        Previous\r
      </button>\r
\r
      @for (page of pages(); track page) {\r
        <button\r
          type="button"\r
          class="btn btn--sm pagination__page"\r
          [class.is-current]="page === pageNumber()"\r
          [attr.aria-current]="page === pageNumber() ? 'page' : null"\r
          [disabled]="disabled()"\r
          (click)="goToPage(page)"\r
        >\r
          {{ page }}\r
        </button>\r
      }\r
\r
      <button\r
        type="button"\r
        class="btn btn--secondary btn--sm"\r
        [disabled]="disabled() || !hasNextPage()"\r
        (click)="goToPage(pageNumber() + 1)"\r
      >\r
        Next\r
      </button>\r
    </div>\r
  </div>\r
</nav>\r
`, styles: ["/* src/app/shared/components/pagination/pagination.scss */\n.pagination {\n  display: flex;\n  flex-wrap: wrap;\n  gap: 1rem;\n  align-items: center;\n  justify-content: space-between;\n  padding: 0.85rem 1.25rem;\n  border-top: 1px solid var(--color-border);\n}\n.pagination__summary {\n  margin: 0;\n  color: var(--color-muted);\n  font-size: 0.875rem;\n}\n.pagination__controls {\n  display: flex;\n  flex-wrap: wrap;\n  align-items: center;\n  gap: 1rem;\n}\n.pagination__size {\n  display: flex;\n  align-items: center;\n  gap: 0.5rem;\n  font-size: 0.875rem;\n  color: var(--color-muted);\n}\n.pagination__pages {\n  display: flex;\n  align-items: center;\n  gap: 0.25rem;\n}\n.pagination__page {\n  min-width: 2.25rem;\n}\n.pagination__page.is-current {\n  background: var(--color-primary);\n  border-color: var(--color-primary);\n  color: #fff;\n}\n/*# sourceMappingURL=pagination.css.map */\n"] }]
  }], null, { pageNumber: [{ type: Input, args: [{ isSignal: true, alias: "pageNumber", required: true }] }], pageSize: [{ type: Input, args: [{ isSignal: true, alias: "pageSize", required: true }] }], totalCount: [{ type: Input, args: [{ isSignal: true, alias: "totalCount", required: true }] }], totalPages: [{ type: Input, args: [{ isSignal: true, alias: "totalPages", required: true }] }], hasPreviousPage: [{ type: Input, args: [{ isSignal: true, alias: "hasPreviousPage", required: true }] }], hasNextPage: [{ type: Input, args: [{ isSignal: true, alias: "hasNextPage", required: true }] }], disabled: [{ type: Input, args: [{ isSignal: true, alias: "disabled", required: false }] }], pageSizeOptions: [{ type: Input, args: [{ isSignal: true, alias: "pageSizeOptions", required: false }] }], pageChange: [{ type: Output, args: ["pageChange"] }], pageSizeChange: [{ type: Output, args: ["pageSizeChange"] }] });
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(Pagination, { className: "Pagination", filePath: "src/app/shared/components/pagination/pagination.ts", lineNumber: 17 });
})();

// src/app/features/employees/employee-list/employee-list.ts
var _c0 = (a0) => ["/employees", a0];
var _c1 = (a0) => ["/employees", a0, "edit"];
var _forTrack0 = ($index, $item) => $item.id;
function EmployeeList_Conditional_14_Template(rf, ctx) {
  if (rf & 1) {
    const _r1 = \u0275\u0275getCurrentView();
    \u0275\u0275elementStart(0, "div", 8)(1, "p", 18);
    \u0275\u0275text(2);
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(3, "button", 19);
    \u0275\u0275listener("click", function EmployeeList_Conditional_14_Template_button_click_3_listener() {
      \u0275\u0275restoreView(_r1);
      const ctx_r1 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r1.dismissNotice());
    });
    \u0275\u0275text(4, "\xD7");
    \u0275\u0275elementEnd()();
  }
  if (rf & 2) {
    \u0275\u0275advance(2);
    \u0275\u0275textInterpolate(ctx);
  }
}
function EmployeeList_Conditional_21_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275elementStart(0, "p", 14);
    \u0275\u0275element(1, "span", 20);
    \u0275\u0275text(2, " Loading employees\u2026 ");
    \u0275\u0275elementEnd();
  }
}
function EmployeeList_Conditional_22_Template(rf, ctx) {
  if (rf & 1) {
    const _r3 = \u0275\u0275getCurrentView();
    \u0275\u0275elementStart(0, "div", 15)(1, "p", 18);
    \u0275\u0275text(2);
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(3, "button", 21);
    \u0275\u0275listener("click", function EmployeeList_Conditional_22_Template_button_click_3_listener() {
      \u0275\u0275restoreView(_r3);
      const ctx_r1 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r1.reload());
    });
    \u0275\u0275text(4, "Try again");
    \u0275\u0275elementEnd()();
  }
  if (rf & 2) {
    \u0275\u0275advance(2);
    \u0275\u0275textInterpolate(ctx);
  }
}
function EmployeeList_Conditional_23_Conditional_1_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275elementStart(0, "p");
    \u0275\u0275text(1);
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(2, "p", 22);
    \u0275\u0275text(3, "Try a different name, mobile number or email address.");
    \u0275\u0275elementEnd();
  }
  if (rf & 2) {
    const ctx_r1 = \u0275\u0275nextContext(2);
    \u0275\u0275advance();
    \u0275\u0275textInterpolate1("No employee matches \u201C", ctx_r1.searchTerm(), "\u201D.");
  }
}
function EmployeeList_Conditional_23_Conditional_2_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275elementStart(0, "p");
    \u0275\u0275text(1, "No employees have been added yet.");
    \u0275\u0275elementEnd();
  }
}
function EmployeeList_Conditional_23_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275elementStart(0, "div", 16);
    \u0275\u0275conditionalCreate(1, EmployeeList_Conditional_23_Conditional_1_Template, 4, 1)(2, EmployeeList_Conditional_23_Conditional_2_Template, 2, 0, "p");
    \u0275\u0275elementStart(3, "a", 7);
    \u0275\u0275text(4, "Add employee");
    \u0275\u0275elementEnd()();
  }
  if (rf & 2) {
    const ctx_r1 = \u0275\u0275nextContext();
    \u0275\u0275advance();
    \u0275\u0275conditional(ctx_r1.searchTerm() ? 1 : 2);
  }
}
function EmployeeList_Conditional_24_For_18_Template(rf, ctx) {
  if (rf & 1) {
    const _r5 = \u0275\u0275getCurrentView();
    \u0275\u0275elementStart(0, "tr")(1, "td", 29);
    \u0275\u0275text(2);
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(3, "td", 30);
    \u0275\u0275text(4);
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(5, "td");
    \u0275\u0275text(6);
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(7, "td");
    \u0275\u0275text(8);
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(9, "td")(10, "span", 31);
    \u0275\u0275text(11);
    \u0275\u0275elementEnd()();
    \u0275\u0275elementStart(12, "td", 32)(13, "a", 33);
    \u0275\u0275text(14, "View");
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(15, "a", 33);
    \u0275\u0275text(16, "Edit");
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(17, "button", 34);
    \u0275\u0275listener("click", function EmployeeList_Conditional_24_For_18_Template_button_click_17_listener() {
      const employee_r6 = \u0275\u0275restoreView(_r5).$implicit;
      const ctx_r1 = \u0275\u0275nextContext(2);
      return \u0275\u0275resetView(ctx_r1.requestDelete(employee_r6));
    });
    \u0275\u0275text(18, "Delete");
    \u0275\u0275elementEnd()()();
  }
  if (rf & 2) {
    const employee_r6 = ctx.$implicit;
    \u0275\u0275advance(2);
    \u0275\u0275textInterpolate(employee_r6.id);
    \u0275\u0275advance(2);
    \u0275\u0275textInterpolate(employee_r6.name);
    \u0275\u0275advance(2);
    \u0275\u0275textInterpolate(employee_r6.mobile);
    \u0275\u0275advance(2);
    \u0275\u0275textInterpolate(employee_r6.email);
    \u0275\u0275advance(2);
    \u0275\u0275classProp("badge--active", employee_r6.isActive)("badge--inactive", !employee_r6.isActive);
    \u0275\u0275advance();
    \u0275\u0275textInterpolate1(" ", employee_r6.isActive ? "Active" : "Inactive", " ");
    \u0275\u0275advance(2);
    \u0275\u0275property("routerLink", \u0275\u0275pureFunction1(11, _c0, employee_r6.id));
    \u0275\u0275advance(2);
    \u0275\u0275property("routerLink", \u0275\u0275pureFunction1(13, _c1, employee_r6.id));
  }
}
function EmployeeList_Conditional_24_Template(rf, ctx) {
  if (rf & 1) {
    const _r4 = \u0275\u0275getCurrentView();
    \u0275\u0275elementStart(0, "div", 23)(1, "table", 24)(2, "thead")(3, "tr")(4, "th", 25);
    \u0275\u0275text(5, "Id");
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(6, "th", 26);
    \u0275\u0275text(7, "Name");
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(8, "th", 26);
    \u0275\u0275text(9, "Mobile");
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(10, "th", 26);
    \u0275\u0275text(11, "Email");
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(12, "th", 26);
    \u0275\u0275text(13, "Status");
    \u0275\u0275elementEnd();
    \u0275\u0275elementStart(14, "th", 27);
    \u0275\u0275text(15, "Actions");
    \u0275\u0275elementEnd()()();
    \u0275\u0275elementStart(16, "tbody");
    \u0275\u0275repeaterCreate(17, EmployeeList_Conditional_24_For_18_Template, 19, 15, "tr", null, _forTrack0);
    \u0275\u0275elementEnd()()();
    \u0275\u0275elementStart(19, "app-pagination", 28);
    \u0275\u0275listener("pageChange", function EmployeeList_Conditional_24_Template_app_pagination_pageChange_19_listener($event) {
      \u0275\u0275restoreView(_r4);
      const ctx_r1 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r1.onPageChange($event));
    })("pageSizeChange", function EmployeeList_Conditional_24_Template_app_pagination_pageSizeChange_19_listener($event) {
      \u0275\u0275restoreView(_r4);
      const ctx_r1 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r1.onPageSizeChange($event));
    });
    \u0275\u0275elementEnd();
  }
  if (rf & 2) {
    const ctx_r1 = \u0275\u0275nextContext();
    \u0275\u0275advance(17);
    \u0275\u0275repeater(ctx_r1.employees());
    \u0275\u0275advance(2);
    \u0275\u0275property("pageNumber", ctx_r1.pageNumber())("pageSize", ctx_r1.pageSize())("totalCount", ctx_r1.totalCount())("totalPages", ctx_r1.totalPages())("hasPreviousPage", ctx_r1.hasPreviousPage())("hasNextPage", ctx_r1.hasNextPage())("disabled", ctx_r1.loading());
  }
}
function EmployeeList_Conditional_25_Template(rf, ctx) {
  if (rf & 1) {
    const _r7 = \u0275\u0275getCurrentView();
    \u0275\u0275elementStart(0, "app-confirm-dialog", 35);
    \u0275\u0275listener("confirmed", function EmployeeList_Conditional_25_Template_app_confirm_dialog_confirmed_0_listener() {
      \u0275\u0275restoreView(_r7);
      const ctx_r1 = \u0275\u0275nextContext();
      return \u0275\u0275resetView(ctx_r1.confirmDelete());
    })("cancelled", function EmployeeList_Conditional_25_Template_app_confirm_dialog_cancelled_0_listener() {
      \u0275\u0275restoreView(_r7);
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
var EmployeeList = class _EmployeeList {
  employeeService = inject(EmployeeService);
  route = inject(ActivatedRoute);
  router = inject(Router);
  destroyRef = inject(DestroyRef);
  /** Raw keystrokes, debounced before they reach the API. */
  searchInput = new Subject();
  /** Every value pushed here issues one request. */
  reloads = new Subject();
  employees = signal(
    [],
    ...ngDevMode ? [{ debugName: "employees" }] : (
      /* istanbul ignore next */
      []
    )
  );
  searchTerm = signal(
    "",
    ...ngDevMode ? [{ debugName: "searchTerm" }] : (
      /* istanbul ignore next */
      []
    )
  );
  pageNumber = signal(
    DEFAULT_PAGE_NUMBER,
    ...ngDevMode ? [{ debugName: "pageNumber" }] : (
      /* istanbul ignore next */
      []
    )
  );
  pageSize = signal(
    DEFAULT_PAGE_SIZE,
    ...ngDevMode ? [{ debugName: "pageSize" }] : (
      /* istanbul ignore next */
      []
    )
  );
  totalCount = signal(
    0,
    ...ngDevMode ? [{ debugName: "totalCount" }] : (
      /* istanbul ignore next */
      []
    )
  );
  totalPages = signal(
    0,
    ...ngDevMode ? [{ debugName: "totalPages" }] : (
      /* istanbul ignore next */
      []
    )
  );
  hasPreviousPage = signal(
    false,
    ...ngDevMode ? [{ debugName: "hasPreviousPage" }] : (
      /* istanbul ignore next */
      []
    )
  );
  hasNextPage = signal(
    false,
    ...ngDevMode ? [{ debugName: "hasNextPage" }] : (
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
  notice = signal(
    null,
    ...ngDevMode ? [{ debugName: "notice" }] : (
      /* istanbul ignore next */
      []
    )
  );
  pendingDelete = signal(
    null,
    ...ngDevMode ? [{ debugName: "pendingDelete" }] : (
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
  showEmptyState = computed(
    () => !this.loading() && this.errorMessage() === null && this.employees().length === 0,
    ...ngDevMode ? [{ debugName: "showEmptyState" }] : (
      /* istanbul ignore next */
      []
    )
  );
  /** Prompt shown by the delete confirmation dialog. */
  deleteMessage = computed(
    () => {
      const employee = this.pendingDelete();
      return `Delete "${employee?.name ?? "this employee"}" permanently? This cannot be undone.`;
    },
    ...ngDevMode ? [{ debugName: "deleteMessage" }] : (
      /* istanbul ignore next */
      []
    )
  );
  constructor() {
    this.route.queryParamMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      if (params.has("created")) {
        this.notice.set("The employee was created.");
      } else if (params.has("updated")) {
        this.notice.set("The employee was updated.");
      } else if (params.has("deleted")) {
        this.notice.set("The employee was deleted.");
      }
    });
    this.searchInput.pipe(debounceTime(SEARCH_DEBOUNCE_MS), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef)).subscribe((term) => {
      this.searchTerm.set(term);
      this.pageNumber.set(DEFAULT_PAGE_NUMBER);
      this.reload();
    });
    this.reloads.pipe(
      tap(() => {
        this.loading.set(true);
        this.errorMessage.set(null);
      }),
      // `catchError` inside `switchMap` keeps the outer pipeline alive after a failure.
      switchMap(() => this.employeeService.getEmployees(this.currentQuery()).pipe(catchError((error) => {
        this.applyLoadFailure(error);
        return EMPTY;
      }))),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe((page) => this.applyPage(page));
    this.reload();
  }
  /** Loads the current page again. */
  reload() {
    this.reloads.next();
  }
  onSearchInput(event) {
    this.searchInput.next(event.target.value);
  }
  onPageChange(page) {
    this.pageNumber.set(page);
    this.reload();
  }
  onPageSizeChange(size) {
    this.pageSize.set(size);
    this.pageNumber.set(DEFAULT_PAGE_NUMBER);
    this.reload();
  }
  dismissNotice() {
    this.notice.set(null);
    void this.router.navigate([], { relativeTo: this.route, queryParams: {}, replaceUrl: true });
  }
  requestDelete(employee) {
    this.pendingDelete.set(employee);
  }
  cancelDelete() {
    if (!this.deleteBusy()) {
      this.pendingDelete.set(null);
    }
  }
  confirmDelete() {
    const employee = this.pendingDelete();
    if (!employee || employee.id === void 0) {
      return;
    }
    this.deleteBusy.set(true);
    this.employeeService.deleteEmployee(employee.id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.deleteBusy.set(false);
        this.pendingDelete.set(null);
        this.notice.set(`"${employee.name ?? "Employee"}" was deleted.`);
        this.afterDelete();
      },
      error: (error) => {
        this.deleteBusy.set(false);
        this.pendingDelete.set(null);
        this.errorMessage.set(errorMessageOf(error));
      }
    });
  }
  afterDelete() {
    if (this.employees().length <= 1 && this.pageNumber() > DEFAULT_PAGE_NUMBER) {
      this.pageNumber.update((page) => page - 1);
    }
    this.reload();
  }
  currentQuery() {
    const search = this.searchTerm().trim();
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      search: search.length > 0 ? search : void 0
    };
  }
  applyPage(page) {
    this.employees.set(page.items ?? []);
    this.totalCount.set(page.totalCount ?? 0);
    this.totalPages.set(page.totalPages ?? 0);
    this.hasPreviousPage.set(page.hasPreviousPage ?? false);
    this.hasNextPage.set(page.hasNextPage ?? false);
    this.loading.set(false);
  }
  applyLoadFailure(error) {
    this.employees.set([]);
    this.totalCount.set(0);
    this.totalPages.set(0);
    this.hasPreviousPage.set(false);
    this.hasNextPage.set(false);
    this.loading.set(false);
    this.errorMessage.set(errorMessageOf(error));
  }
  static \u0275fac = function EmployeeList_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _EmployeeList)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _EmployeeList, selectors: [["app-employee-list"]], decls: 26, vars: 7, consts: [[1, "page"], [1, "page__header"], [1, "page__eyebrow"], [1, "page__title"], [1, "page__subtitle"], [1, "page__actions"], ["type", "button", 1, "btn", "btn--secondary", 3, "click", "disabled"], ["routerLink", "/employees/new", 1, "btn", "btn--primary"], ["role", "status", 1, "alert", "alert--success"], [1, "card"], [1, "card__toolbar"], [1, "field", "field--search"], ["for", "employee-search", 1, "field__label"], ["id", "employee-search", "type", "search", "placeholder", "Name, mobile or email", "autocomplete", "off", 1, "field__control", 3, "input", "value"], ["role", "status", 1, "status-line"], ["role", "alert", 1, "alert", "alert--error"], [1, "empty-state"], ["title", "Delete employee", "confirmLabel", "Delete", 3, "message", "busy"], [1, "alert__message"], ["type", "button", "aria-label", "Dismiss message", 1, "alert__dismiss", 3, "click"], ["aria-hidden", "true", 1, "spinner", "spinner--sm"], ["type", "button", 1, "btn", "btn--secondary", "btn--sm", 3, "click"], [1, "empty-state__hint"], [1, "table-wrapper"], [1, "table"], ["scope", "col", 1, "table__id"], ["scope", "col"], ["scope", "col", 1, "table__actions-header"], [3, "pageChange", "pageSizeChange", "pageNumber", "pageSize", "totalCount", "totalPages", "hasPreviousPage", "hasNextPage", "disabled"], [1, "table__id"], [1, "table__primary"], [1, "badge"], [1, "table__actions"], [1, "btn", "btn--ghost", "btn--sm", 3, "routerLink"], ["type", "button", 1, "btn", "btn--ghost", "btn--sm", "btn--danger-text", 3, "click"], ["title", "Delete employee", "confirmLabel", "Delete", 3, "confirmed", "cancelled", "message", "busy"]], template: function EmployeeList_Template(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275elementStart(0, "section", 0)(1, "header", 1)(2, "div")(3, "p", 2);
      \u0275\u0275text(4, "Directory");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(5, "h1", 3);
      \u0275\u0275text(6, "Employees");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(7, "p", 4);
      \u0275\u0275text(8, "Browse, search and manage every employee record.");
      \u0275\u0275elementEnd()();
      \u0275\u0275elementStart(9, "div", 5)(10, "button", 6);
      \u0275\u0275listener("click", function EmployeeList_Template_button_click_10_listener() {
        return ctx.reload();
      });
      \u0275\u0275text(11, "Refresh");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(12, "a", 7);
      \u0275\u0275text(13, "Add employee");
      \u0275\u0275elementEnd()()();
      \u0275\u0275conditionalCreate(14, EmployeeList_Conditional_14_Template, 5, 1, "div", 8);
      \u0275\u0275elementStart(15, "div", 9)(16, "div", 10)(17, "div", 11)(18, "label", 12);
      \u0275\u0275text(19, "Search");
      \u0275\u0275elementEnd();
      \u0275\u0275elementStart(20, "input", 13);
      \u0275\u0275listener("input", function EmployeeList_Template_input_input_20_listener($event) {
        return ctx.onSearchInput($event);
      });
      \u0275\u0275elementEnd()();
      \u0275\u0275conditionalCreate(21, EmployeeList_Conditional_21_Template, 3, 0, "p", 14);
      \u0275\u0275elementEnd();
      \u0275\u0275conditionalCreate(22, EmployeeList_Conditional_22_Template, 5, 1, "div", 15);
      \u0275\u0275conditionalCreate(23, EmployeeList_Conditional_23_Template, 5, 1, "div", 16)(24, EmployeeList_Conditional_24_Template, 20, 7);
      \u0275\u0275elementEnd()();
      \u0275\u0275conditionalCreate(25, EmployeeList_Conditional_25_Template, 1, 2, "app-confirm-dialog", 17);
    }
    if (rf & 2) {
      let tmp_1_0;
      let tmp_4_0;
      \u0275\u0275advance(10);
      \u0275\u0275property("disabled", ctx.loading());
      \u0275\u0275advance(4);
      \u0275\u0275conditional((tmp_1_0 = ctx.notice()) ? 14 : -1, tmp_1_0);
      \u0275\u0275advance(6);
      \u0275\u0275property("value", ctx.searchTerm());
      \u0275\u0275advance();
      \u0275\u0275conditional(ctx.loading() ? 21 : -1);
      \u0275\u0275advance();
      \u0275\u0275conditional((tmp_4_0 = ctx.errorMessage()) ? 22 : -1, tmp_4_0);
      \u0275\u0275advance();
      \u0275\u0275conditional(ctx.showEmptyState() ? 23 : ctx.employees().length > 0 ? 24 : -1);
      \u0275\u0275advance(2);
      \u0275\u0275conditional(ctx.pendingDelete() ? 25 : -1);
    }
  }, dependencies: [RouterLink, Pagination, ConfirmDialog], styles: ["\n.field--search[_ngcontent-%COMP%] {\n  max-width: 22rem;\n  width: 100%;\n}\n.table__id[_ngcontent-%COMP%] {\n  width: 4.5rem;\n  color: var(--%NS%color-muted);\n}\n.table__actions-header[_ngcontent-%COMP%] {\n  text-align: right;\n}\n/*# sourceMappingURL=employee-list.css.map */"] });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(EmployeeList, [{
    type: Component,
    args: [{ selector: "app-employee-list", imports: [RouterLink, Pagination, ConfirmDialog], changeDetection: ChangeDetectionStrategy.OnPush, template: `<section class="page">\r
  <header class="page__header">\r
    <div>\r
      <p class="page__eyebrow">Directory</p>\r
      <h1 class="page__title">Employees</h1>\r
      <p class="page__subtitle">Browse, search and manage every employee record.</p>\r
    </div>\r
\r
    <div class="page__actions">\r
      <button type="button" class="btn btn--secondary" [disabled]="loading()" (click)="reload()">Refresh</button>\r
      <a class="btn btn--primary" routerLink="/employees/new">Add employee</a>\r
    </div>\r
  </header>\r
\r
  @if (notice(); as message) {\r
    <div class="alert alert--success" role="status">\r
      <p class="alert__message">{{ message }}</p>\r
      <button type="button" class="alert__dismiss" aria-label="Dismiss message" (click)="dismissNotice()">&times;</button>\r
    </div>\r
  }\r
\r
  <div class="card">\r
    <div class="card__toolbar">\r
      <div class="field field--search">\r
        <label class="field__label" for="employee-search">Search</label>\r
        <input\r
          id="employee-search"\r
          type="search"\r
          class="field__control"\r
          placeholder="Name, mobile or email"\r
          autocomplete="off"\r
          [value]="searchTerm()"\r
          (input)="onSearchInput($event)"\r
        />\r
      </div>\r
\r
      @if (loading()) {\r
        <p class="status-line" role="status">\r
          <span class="spinner spinner--sm" aria-hidden="true"></span>\r
          Loading employees&hellip;\r
        </p>\r
      }\r
    </div>\r
\r
    @if (errorMessage(); as message) {\r
      <div class="alert alert--error" role="alert">\r
        <p class="alert__message">{{ message }}</p>\r
        <button type="button" class="btn btn--secondary btn--sm" (click)="reload()">Try again</button>\r
      </div>\r
    }\r
\r
    @if (showEmptyState()) {\r
      <div class="empty-state">\r
        @if (searchTerm()) {\r
          <p>No employee matches &ldquo;{{ searchTerm() }}&rdquo;.</p>\r
          <p class="empty-state__hint">Try a different name, mobile number or email address.</p>\r
        } @else {\r
          <p>No employees have been added yet.</p>\r
        }\r
\r
        <a class="btn btn--primary" routerLink="/employees/new">Add employee</a>\r
      </div>\r
    } @else if (employees().length > 0) {\r
      <div class="table-wrapper">\r
        <table class="table">\r
          <thead>\r
            <tr>\r
              <th scope="col" class="table__id">Id</th>\r
              <th scope="col">Name</th>\r
              <th scope="col">Mobile</th>\r
              <th scope="col">Email</th>\r
              <th scope="col">Status</th>\r
              <th scope="col" class="table__actions-header">Actions</th>\r
            </tr>\r
          </thead>\r
          <tbody>\r
            @for (employee of employees(); track employee.id) {\r
              <tr>\r
                <td class="table__id">{{ employee.id }}</td>\r
                <td class="table__primary">{{ employee.name }}</td>\r
                <td>{{ employee.mobile }}</td>\r
                <td>{{ employee.email }}</td>\r
                <td>\r
                  <span class="badge" [class.badge--active]="employee.isActive" [class.badge--inactive]="!employee.isActive">\r
                    {{ employee.isActive ? 'Active' : 'Inactive' }}\r
                  </span>\r
                </td>\r
                <td class="table__actions">\r
                  <a class="btn btn--ghost btn--sm" [routerLink]="['/employees', employee.id]">View</a>\r
                  <a class="btn btn--ghost btn--sm" [routerLink]="['/employees', employee.id, 'edit']">Edit</a>\r
                  <button type="button" class="btn btn--ghost btn--sm btn--danger-text" (click)="requestDelete(employee)">Delete</button>\r
                </td>\r
              </tr>\r
            }\r
          </tbody>\r
        </table>\r
      </div>\r
\r
      <app-pagination\r
        [pageNumber]="pageNumber()"\r
        [pageSize]="pageSize()"\r
        [totalCount]="totalCount()"\r
        [totalPages]="totalPages()"\r
        [hasPreviousPage]="hasPreviousPage()"\r
        [hasNextPage]="hasNextPage()"\r
        [disabled]="loading()"\r
        (pageChange)="onPageChange($event)"\r
        (pageSizeChange)="onPageSizeChange($event)"\r
      />\r
    }\r
  </div>\r
</section>\r
\r
@if (pendingDelete()) {\r
  <app-confirm-dialog\r
    title="Delete employee"\r
    [message]="deleteMessage()"\r
    confirmLabel="Delete"\r
    [busy]="deleteBusy()"\r
    (confirmed)="confirmDelete()"\r
    (cancelled)="cancelDelete()"\r
  />\r
}\r
`, styles: ["/* src/app/features/employees/employee-list/employee-list.scss */\n.field--search {\n  max-width: 22rem;\n  width: 100%;\n}\n.table__id {\n  width: 4.5rem;\n  color: var(--color-muted);\n}\n.table__actions-header {\n  text-align: right;\n}\n/*# sourceMappingURL=employee-list.css.map */\n"] }]
  }], () => [], null);
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(EmployeeList, { className: "EmployeeList", filePath: "src/app/features/employees/employee-list/employee-list.ts", lineNumber: 32 });
})();
export {
  EmployeeList
};
//# debugId=c06d0a32-9b7e-5f3a-8f1f-a5c2da2903ec
//# sourceMappingURL=chunk-UCJWJEHN.js.map
