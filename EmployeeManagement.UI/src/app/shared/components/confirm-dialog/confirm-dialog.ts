import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

/**
 * Modal confirmation prompt used before destructive actions.
 *
 * Rendered conditionally by the parent (`@if (pendingDelete())`), so it also serves as the
 * accessible focus container: `role="alertdialog"` plus an Escape handler.
 */
@Component({
  selector: 'app-confirm-dialog',
  imports: [],
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '(document:keydown.escape)': 'onCancel()',
  },
})
export class ConfirmDialog {
  readonly title = input('Please confirm');
  readonly message = input.required<string>();
  readonly confirmLabel = input('Confirm');
  readonly cancelLabel = input('Cancel');

  /** True while the confirmed action is running; keeps both buttons disabled. */
  readonly busy = input(false);

  readonly confirmed = output<void>();
  readonly cancelled = output<void>();

  protected onConfirm(): void {
    if (!this.busy()) {
      this.confirmed.emit();
    }
  }

  protected onCancel(): void {
    if (!this.busy()) {
      this.cancelled.emit();
    }
  }

  protected onBackdropClick(event: MouseEvent): void {
    if (event.target === event.currentTarget) {
      this.onCancel();
    }
  }
}
