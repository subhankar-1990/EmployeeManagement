import {
  ChangeDetectionStrategy,
  Component,
  Input,
  Output,
  input,
  output,
  setClassMetadata,
  ɵsetClassDebugInfo,
  ɵɵadvance,
  ɵɵattribute,
  ɵɵconditional,
  ɵɵconditionalCreate,
  ɵɵdefineComponent,
  ɵɵdomElement,
  ɵɵdomElementEnd,
  ɵɵdomElementStart,
  ɵɵdomListener,
  ɵɵdomProperty,
  ɵɵlistener,
  ɵɵresolveDocument,
  ɵɵtext,
  ɵɵtextInterpolate,
  ɵɵtextInterpolate1
} from "./chunk-3KAAG4ND.js";

// src/app/shared/components/confirm-dialog/confirm-dialog.ts
function ConfirmDialog_Conditional_10_Template(rf, ctx) {
  if (rf & 1) {
    \u0275\u0275domElement(0, "span", 7);
  }
}
var ConfirmDialog = class _ConfirmDialog {
  title = input(
    "Please confirm",
    ...ngDevMode ? [{ debugName: "title" }] : (
      /* istanbul ignore next */
      []
    )
  );
  message = input.required(
    ...ngDevMode ? [{ debugName: "message" }] : (
      /* istanbul ignore next */
      []
    )
  );
  confirmLabel = input(
    "Confirm",
    ...ngDevMode ? [{ debugName: "confirmLabel" }] : (
      /* istanbul ignore next */
      []
    )
  );
  cancelLabel = input(
    "Cancel",
    ...ngDevMode ? [{ debugName: "cancelLabel" }] : (
      /* istanbul ignore next */
      []
    )
  );
  /** True while the confirmed action is running; keeps both buttons disabled. */
  busy = input(
    false,
    ...ngDevMode ? [{ debugName: "busy" }] : (
      /* istanbul ignore next */
      []
    )
  );
  confirmed = output();
  cancelled = output();
  onConfirm() {
    if (!this.busy()) {
      this.confirmed.emit();
    }
  }
  onCancel() {
    if (!this.busy()) {
      this.cancelled.emit();
    }
  }
  onBackdropClick(event) {
    if (event.target === event.currentTarget) {
      this.onCancel();
    }
  }
  static \u0275fac = function ConfirmDialog_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _ConfirmDialog)();
  };
  static \u0275cmp = /* @__PURE__ */ \u0275\u0275defineComponent({ type: _ConfirmDialog, selectors: [["app-confirm-dialog"]], hostBindings: function ConfirmDialog_HostBindings(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275listener("keydown.escape", function ConfirmDialog_keydown_escape_HostBindingHandler() {
        return ctx.onCancel();
      }, \u0275\u0275resolveDocument);
    }
  }, inputs: { title: [1, "title"], message: [1, "message"], confirmLabel: [1, "confirmLabel"], cancelLabel: [1, "cancelLabel"], busy: [1, "busy"] }, outputs: { confirmed: "confirmed", cancelled: "cancelled" }, decls: 13, vars: 8, consts: [[1, "dialog-backdrop", 3, "click"], ["role", "alertdialog", "aria-modal", "true", 1, "dialog"], [1, "dialog__title"], [1, "dialog__message"], [1, "dialog__actions"], ["type", "button", 1, "btn", "btn--secondary", 3, "click", "disabled"], ["type", "button", 1, "btn", "btn--danger", 3, "click", "disabled"], ["aria-hidden", "true", 1, "spinner", "spinner--sm"]], template: function ConfirmDialog_Template(rf, ctx) {
    if (rf & 1) {
      \u0275\u0275domElementStart(0, "div", 0);
      \u0275\u0275domListener("click", function ConfirmDialog_Template_div_click_0_listener($event) {
        return ctx.onBackdropClick($event);
      });
      \u0275\u0275domElementStart(1, "div", 1)(2, "h2", 2);
      \u0275\u0275text(3);
      \u0275\u0275domElementEnd();
      \u0275\u0275domElementStart(4, "p", 3);
      \u0275\u0275text(5);
      \u0275\u0275domElementEnd();
      \u0275\u0275domElementStart(6, "div", 4)(7, "button", 5);
      \u0275\u0275domListener("click", function ConfirmDialog_Template_button_click_7_listener() {
        return ctx.onCancel();
      });
      \u0275\u0275text(8);
      \u0275\u0275domElementEnd();
      \u0275\u0275domElementStart(9, "button", 6);
      \u0275\u0275domListener("click", function ConfirmDialog_Template_button_click_9_listener() {
        return ctx.onConfirm();
      });
      \u0275\u0275conditionalCreate(10, ConfirmDialog_Conditional_10_Template, 1, 0, "span", 7);
      \u0275\u0275domElementStart(11, "span");
      \u0275\u0275text(12);
      \u0275\u0275domElementEnd()()()()();
    }
    if (rf & 2) {
      \u0275\u0275advance();
      \u0275\u0275attribute("aria-label", ctx.title());
      \u0275\u0275advance(2);
      \u0275\u0275textInterpolate(ctx.title());
      \u0275\u0275advance(2);
      \u0275\u0275textInterpolate(ctx.message());
      \u0275\u0275advance(2);
      \u0275\u0275domProperty("disabled", ctx.busy());
      \u0275\u0275advance();
      \u0275\u0275textInterpolate1(" ", ctx.cancelLabel(), " ");
      \u0275\u0275advance();
      \u0275\u0275domProperty("disabled", ctx.busy());
      \u0275\u0275advance();
      \u0275\u0275conditional(ctx.busy() ? 10 : -1);
      \u0275\u0275advance(2);
      \u0275\u0275textInterpolate(ctx.confirmLabel());
    }
  }, styles: ["\n.dialog-backdrop[_ngcontent-%COMP%] {\n  position: fixed;\n  inset: 0;\n  z-index: 50;\n  display: flex;\n  align-items: center;\n  justify-content: center;\n  padding: 1.5rem;\n  background: rgba(15, 20, 35, 0.45);\n}\n.dialog[_ngcontent-%COMP%] {\n  width: 100%;\n  max-width: 26rem;\n  padding: 1.5rem;\n  border-radius: var(--%NS%radius-lg);\n  background: var(--%NS%color-surface);\n  box-shadow: var(--%NS%shadow-md);\n}\n.dialog__title[_ngcontent-%COMP%] {\n  margin: 0;\n  font-size: 1.125rem;\n  font-weight: 600;\n}\n.dialog__message[_ngcontent-%COMP%] {\n  margin: 0.5rem 0 0;\n  color: var(--%NS%color-muted);\n  font-size: 0.9375rem;\n}\n.dialog__actions[_ngcontent-%COMP%] {\n  display: flex;\n  justify-content: flex-end;\n  gap: 0.75rem;\n  margin-top: 1.5rem;\n}\n/*# sourceMappingURL=confirm-dialog.css.map */"] });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(ConfirmDialog, [{
    type: Component,
    args: [{ selector: "app-confirm-dialog", imports: [], changeDetection: ChangeDetectionStrategy.OnPush, host: {
      "(document:keydown.escape)": "onCancel()"
    }, template: '<div class="dialog-backdrop" (click)="onBackdropClick($event)">\r\n  <div class="dialog" role="alertdialog" aria-modal="true" [attr.aria-label]="title()">\r\n    <h2 class="dialog__title">{{ title() }}</h2>\r\n    <p class="dialog__message">{{ message() }}</p>\r\n\r\n    <div class="dialog__actions">\r\n      <button type="button" class="btn btn--secondary" [disabled]="busy()" (click)="onCancel()">\r\n        {{ cancelLabel() }}\r\n      </button>\r\n      <button type="button" class="btn btn--danger" [disabled]="busy()" (click)="onConfirm()">\r\n        @if (busy()) {\r\n          <span class="spinner spinner--sm" aria-hidden="true"></span>\r\n        }\r\n        <span>{{ confirmLabel() }}</span>\r\n      </button>\r\n    </div>\r\n  </div>\r\n</div>\r\n', styles: ["/* src/app/shared/components/confirm-dialog/confirm-dialog.scss */\n.dialog-backdrop {\n  position: fixed;\n  inset: 0;\n  z-index: 50;\n  display: flex;\n  align-items: center;\n  justify-content: center;\n  padding: 1.5rem;\n  background: rgba(15, 20, 35, 0.45);\n}\n.dialog {\n  width: 100%;\n  max-width: 26rem;\n  padding: 1.5rem;\n  border-radius: var(--radius-lg);\n  background: var(--color-surface);\n  box-shadow: var(--shadow-md);\n}\n.dialog__title {\n  margin: 0;\n  font-size: 1.125rem;\n  font-weight: 600;\n}\n.dialog__message {\n  margin: 0.5rem 0 0;\n  color: var(--color-muted);\n  font-size: 0.9375rem;\n}\n.dialog__actions {\n  display: flex;\n  justify-content: flex-end;\n  gap: 0.75rem;\n  margin-top: 1.5rem;\n}\n/*# sourceMappingURL=confirm-dialog.css.map */\n"] }]
  }], null, { title: [{ type: Input, args: [{ isSignal: true, alias: "title", required: false }] }], message: [{ type: Input, args: [{ isSignal: true, alias: "message", required: true }] }], confirmLabel: [{ type: Input, args: [{ isSignal: true, alias: "confirmLabel", required: false }] }], cancelLabel: [{ type: Input, args: [{ isSignal: true, alias: "cancelLabel", required: false }] }], busy: [{ type: Input, args: [{ isSignal: true, alias: "busy", required: false }] }], confirmed: [{ type: Output, args: ["confirmed"] }], cancelled: [{ type: Output, args: ["cancelled"] }] });
})();
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && \u0275setClassDebugInfo(ConfirmDialog, { className: "ConfirmDialog", filePath: "src/app/shared/components/confirm-dialog/confirm-dialog.ts", lineNumber: 19 });
})();

export {
  ConfirmDialog
};
//# debugId=8c3ee0d7-2e46-5d3c-b7e7-863f2f514fe5
//# sourceMappingURL=chunk-554XGCVP.js.map
