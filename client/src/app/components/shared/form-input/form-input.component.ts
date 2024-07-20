import { Component, Input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { NgClass, NgIf } from '@angular/common';

@Component({
    selector: 'app-form-input',
    standalone: true,
    imports: [
        ReactiveFormsModule,
        NgClass,
        NgIf
    ],
    templateUrl: './form-input.component.html',
    styleUrl: './form-input.component.css',
})
export class FormInputComponent {
    @Input() formGroup!: FormGroup;
    @Input() controlName!: string;
    @Input() label: string = '';
    @Input() type: string = 'text';
    @Input() required: boolean = false;
    @Input() errorMessages: { [key: string]: string } = {};

    isTouched = false;

    get control() {
        return this.formGroup.get(this.controlName);
    }

    get isInvalid(): boolean {
        return !!this.control && this.control.invalid && this.control.dirty && this.isTouched;
    }

    get errorMessage(): string | null {
        if (!this.control || !this.control.errors) {
            return null;
        }

        for (const error in this.control.errors) {
            if (this.control.errors.hasOwnProperty(error)) {
                return this.errorMessages[error] || 'Invalid field';
            }
        }

        return null;
    }

    onBlur() {
        this.isTouched = true;
    }
}
