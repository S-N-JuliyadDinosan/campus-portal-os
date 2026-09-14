import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { FeeService } from '../../core/services/portal-services';
import { FeePayment } from '../../core/models/domain.models';
import { StatusBadgeComponent } from '../../shared/components/status-badge.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { ApiErrorService } from '../../core/services/api-error.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-fees',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, StatusBadgeComponent, EmptyStateComponent],
  template: `
    <div class="page">
      <div class="page-head"><div><h1>Fees & payments</h1><p>Review outstanding university fees, simulate payment and open printable receipts.</p></div></div>

      <div class="grid grid-3" style="margin-bottom:18px">
        <div class="card stat"><div class="stat-icon">LKR</div><div><div class="stat-value">{{outstandingAmount|number:'1.0-0'}}</div><div class="stat-label">Outstanding balance</div></div></div>
        <div class="card stat"><div class="stat-icon">✓</div><div><div class="stat-value">{{paidAmount|number:'1.0-0'}}</div><div class="stat-label">Paid total</div></div></div>
        <div class="card stat"><div class="stat-icon">#</div><div><div class="stat-value">{{fees.length}}</div><div class="stat-label">Fee items</div></div></div>
      </div>

      <div class="card">
        <div class="row between"><div><h3 class="card-title">My fee statement</h3><div class="card-sub">Payments are simulated by the backend; duplicate payment is prevented server-side.</div></div><button class="btn btn-secondary btn-sm" (click)="load()">Refresh</button></div>
        <div class="divider"></div>
        @if (fees.length) {
          <div class="table-wrap"><table class="table"><thead><tr><th>Fee ID</th><th>Fee type</th><th>Billing period</th><th>Due</th><th>Amount</th><th>Status</th><th>Receipt</th><th></th></tr></thead><tbody>
            @for (f of fees; track f.feePaymentId) {
              <tr>
                <td>{{f.feePaymentId}}</td><td><strong>{{f.feeTypeName}}</strong></td>
                <td>{{f.billingPeriod}}</td><td>{{f.dueDate|date:'mediumDate'}}</td><td><strong>{{f.amount|currency:'LKR ':'symbol':'1.2-2'}}</strong></td>
                <td><app-status-badge [value]="f.status"/></td>
                <td>@if (f.status === 'Paid' || f.receiptNumber) {<a class="btn btn-secondary btn-sm" [routerLink]="['/fees',f.feePaymentId,'receipt']">Receipt</a>} @else {—}</td>
                <td class="right">@if (f.status === 'Outstanding') {<button class="btn btn-primary btn-sm" [disabled]="payingId === f.feePaymentId" (click)="openPayment(f)">Pay now</button>}</td>
              </tr>
            }
          </tbody></table></div>
        } @else {
          <app-empty-state icon="◫" title="No fee items" message="Your assigned fees and payment history will appear here."/>
        }
      </div>

      @if (paymentModal) {
        <div class="modal-backdrop" (click)="closePayment()">
          <div class="modal" (click)="$event.stopPropagation()">
            <div class="modal-head">
              <div><h2>Confirm payment</h2><div class="small muted">Review the fee before submitting your payment.</div></div>
              <button class="icon-btn" type="button" [disabled]="payingId !== null" (click)="closePayment()">×</button>
            </div>
            <form class="stack" #paymentForm="ngForm" (ngSubmit)="submitPayment()">
              <div class="card" style="padding:16px;background:#f8fafc;box-shadow:none">
                <div class="small muted">Fee</div><strong>{{paymentModal.feeTypeName}}</strong>
                <div class="divider" style="margin:14px 0"></div>
                <div class="row between"><span class="muted">Billing period</span><strong>{{paymentModal.billingPeriod}}</strong></div>
                <div class="row between" style="margin-top:10px"><span class="muted">Amount</span><strong style="font-size:1.25rem">{{paymentModal.amount|currency:'LKR ':'symbol':'1.2-2'}}</strong></div>
              </div>
              <div class="alert info">This is a demo payment. No external payment gateway or real money transfer is used.</div>
              <label class="row" style="align-items:flex-start;gap:10px"><input type="checkbox" name="acknowledged" required [(ngModel)]="paymentAcknowledged"> <span>I have reviewed the amount and want to submit this payment.</span></label>
              <div class="row"><button class="btn btn-primary" [disabled]="paymentForm.invalid || payingId !== null">{{payingId !== null ? 'Processing…' : 'Confirm payment'}}</button><button type="button" class="btn btn-secondary" [disabled]="payingId !== null" (click)="closePayment()">Cancel</button></div>
            </form>
          </div>
        </div>
      }
    </div>
  `
})
export class FeesComponent implements OnInit {
  fees: FeePayment[] = [];
  payingId: number | null = null;
  paymentModal: FeePayment | null = null;
  paymentAcknowledged = false;

  constructor(private service: FeeService, private errors: ApiErrorService, private toast: ToastService) {}

  ngOnInit() { this.load(); }
  get outstandingAmount() { return this.fees.filter(f => f.status === 'Outstanding').reduce((sum, fee) => sum + Number(fee.amount), 0); }
  get paidAmount() { return this.fees.filter(f => f.status === 'Paid').reduce((sum, fee) => sum + Number(fee.amount), 0); }
  load() { this.service.mine().subscribe({ next: fees => this.fees = fees, error: error => this.toast.error(this.errors.message(error)) }); }
  openPayment(fee: FeePayment) { this.paymentModal = fee; this.paymentAcknowledged = false; }
  closePayment() { if (this.payingId === null) { this.paymentModal = null; this.paymentAcknowledged = false; } }
  submitPayment() {
    const fee = this.paymentModal;
    if (!fee || !this.paymentAcknowledged) return;
    this.payingId = fee.feePaymentId;
    this.service.pay(fee.feePaymentId).subscribe({
      next: () => { this.payingId = null; this.paymentModal = null; this.paymentAcknowledged = false; this.toast.success('Demo payment completed.'); this.load(); },
      error: error => { this.payingId = null; this.toast.error(this.errors.message(error)); }
    });
  }
}
