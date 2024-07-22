import { Component, Input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { NgClass, NgIf } from '@angular/common';

/**
 * `FormInputComponent` is a reusable Angular component that encapsulates the behavior and presentation
 * of a form input. It supports various input types (e.g., text, password), validation, and custom error messages.
 * It integrates with Angular's reactive forms module for form control and validation.
 */
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
    @Input() placeholderText: string = '';

    isTouched = false;

    /**
     * Returns the form control object for the specified control name.
     */
    get control() {
        return this.formGroup.get(this.controlName);
    }

    /**
     * Determines if the form control is invalid and has been touched.
     * This is used to show validation errors only after the user has interacted with the input.
     */
    get isInvalid(): boolean {
        return !!this.control && this.control.invalid && this.control.dirty && this.isTouched;
    }

    /**
     * Computes the error message to display based on the current validation errors of the form control.
     * It uses the custom error messages provided through the `errorMessages` input if available.
     */
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

    /**
     * Marks the input as touched when it loses focus. This is used to trigger validation error display.
     */
    onBlur() {
        this.isTouched = true;
    }
}
