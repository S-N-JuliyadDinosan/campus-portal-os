import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FeeService } from '../../data-access/fee.service';
import { FeeReceipt } from '../../models/fee.models';
import { ApiErrorService } from '../../../../core/http/api-error.service';

@Component({
  selector: 'app-receipt',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page">
      <div class="page-head no-print">
        <div><h1>Payment receipt</h1><p>Printable receipt generated from the campus API.</p></div>
        <div class="row"><a class="btn btn-secondary" routerLink="/fees">Back</a><button class="btn btn-secondary" [disabled]="!receipt" (click)="downloadPdf()">Download PDF</button><button class="btn btn-primary" [disabled]="!receipt" (click)="print()">Print receipt</button></div>
      </div>

      @if (error) {
        <div class="alert error">{{error}}</div>
      } @else if (!receipt) {
        <div class="empty">Loading receipt…</div>
      } @else {
        <article class="receipt">
          <div class="receipt-head"><div><div style="font-weight:800;font-family:Manrope;font-size:1.4rem">Campus Services Portal</div><div class="muted small">Official fee payment receipt</div></div><div class="right"><strong>{{receipt.receiptNumber}}</strong><div class="muted small">{{receipt.paidAt|date:'medium'}}</div></div></div>
          <div class="grid grid-2" style="margin-top:26px">
            <div><div class="muted small">Student ID</div><strong>{{receipt.studentId}}</strong></div>
            <div><div class="muted small">Fee payment ID</div><strong>{{receipt.feePaymentId}}</strong></div>
            <div><div class="muted small">Fee type</div><strong>{{receipt.feeType}}</strong></div>
            <div><div class="muted small">Billing period</div><strong>{{receipt.billingPeriod}}</strong></div>
            <div><div class="muted small">Payment method</div><strong>{{receipt.paymentMethod}}</strong></div>
            <div><div class="muted small">Reference</div><strong class="mono">{{receipt.paymentReference}}</strong></div>
          </div>
          <div class="divider"></div>
          <div class="row between"><div><div class="muted small">Amount paid</div><h2 style="margin-top:5px">{{receipt.amount|currency:'LKR ':'symbol':'1.2-2'}}</h2></div><span class="badge success">Paid</span></div>
          <div class="alert info" style="margin-top:24px">This project uses a simulated payment flow. No external payment gateway transaction is performed.</div>
        </article>
      }
    </div>
  `
})
export class ReceiptComponent implements OnInit {
  receipt: FeeReceipt | null = null;
  error = '';

  constructor(private route: ActivatedRoute, private service: FeeService, private errors: ApiErrorService) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.service.receipt(id).subscribe({ next: receipt => this.receipt = receipt, error: error => this.error = this.errors.message(error) });
  }

  print() { window.print(); }

  downloadPdf() {
    if (!this.receipt) return;

    const receipt = this.receipt;
    const paidAt = new Date(receipt.paidAt).toLocaleString();
    const lines = [
      ['Campus Services Portal', 18],
      ['Official fee payment receipt', 11],
      ['', 10],
      [`Receipt number: ${receipt.receiptNumber}`, 11],
      [`Paid at: ${paidAt}`, 11],
      ['', 10],
      [`Student ID: ${receipt.studentId}`, 11],
      [`Fee payment ID: ${receipt.feePaymentId}`, 11],
      [`Fee type: ${receipt.feeType}`, 11],
      [`Billing period: ${receipt.billingPeriod}`, 11],
      [`Payment method: ${receipt.paymentMethod}`, 11],
      [`Reference: ${receipt.paymentReference}`, 10],
      ['', 10],
      [`Amount paid: LKR ${Number(receipt.amount).toFixed(2)}`, 16],
      ['', 10],
      ['Payment status: Paid', 11],
      ['This is a simulated payment receipt. No external transaction was processed.', 9]
    ] as const;

    const content = this.createPdfContent(lines);
    const pdf = this.createPdf(content);
    const url = URL.createObjectURL(new Blob([pdf], { type: 'application/pdf' }));
    const link = document.createElement('a');
    link.href = url;
    link.download = `receipt-${this.fileSafe(receipt.receiptNumber || String(receipt.feePaymentId))}.pdf`;
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.setTimeout(() => URL.revokeObjectURL(url), 1000);
  }

  private createPdfContent(lines: readonly (readonly [string, number])[]) {
    const commands = ['BT', '/F1 18 Tf', '50 790 Td'];
    lines.forEach(([text, size], index) => {
      if (index > 0) commands.push('0 -25 Td');
      commands.push(`/F1 ${size} Tf`, `(${this.pdfText(text)}) Tj`);
    });
    commands.push('ET');
    return commands.join('\n');
  }

  private createPdf(content: string) {
    const objects = [
      '<< /Type /Catalog /Pages 2 0 R >>',
      '<< /Type /Pages /Kids [3 0 R] /Count 1 >>',
      '<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>',
      `<< /Length ${content.length} >>\nstream\n${content}\nendstream`,
      '<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>'
    ];
    let pdf = '%PDF-1.4\n';
    const offsets = [0];
    objects.forEach((object, index) => {
      offsets.push(pdf.length);
      pdf += `${index + 1} 0 obj\n${object}\nendobj\n`;
    });
    const xrefOffset = pdf.length;
    pdf += `xref\n0 ${objects.length + 1}\n0000000000 65535 f \n`;
    offsets.slice(1).forEach(offset => pdf += `${String(offset).padStart(10, '0')} 00000 n \n`);
    pdf += `trailer\n<< /Size ${objects.length + 1} /Root 1 0 R >>\nstartxref\n${xrefOffset}\n%%EOF`;
    return pdf;
  }

  private pdfText(value: string) {
    return value.replace(/[^\x20-\x7E]/g, '?').replace(/([\\()])/g, '\\$1');
  }

  private fileSafe(value: string) {
    return value.replace(/[^a-z0-9_-]/gi, '-');
  }
}
