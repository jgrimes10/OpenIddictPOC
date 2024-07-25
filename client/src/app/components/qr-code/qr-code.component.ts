import { Component, inject, Input } from '@angular/core';
import { QrCodeService } from "../../services/qr-code/qr-code.service";
import { NgIf } from "@angular/common";

@Component({
  selector: 'app-qr-code',
  standalone: true,
    imports: [
        NgIf
    ],
  templateUrl: './qr-code.component.html',
  styleUrl: './qr-code.component.css'
})
export class QrCodeComponent {
    @Input() data: string = "";
    qrCodeImage: string = "";

    private qrCodeService: QrCodeService = inject(QrCodeService);

    ngOnInit(): void {
        this.generateQrCode();
    }

    generateQrCode(): void {
        this.qrCodeService.generateQRCode(this.data).then(qrCode => {
            this.qrCodeImage = qrCode;
        }).catch(err => {
            console.error('Error generating QR code', err);
        });
    }
}
