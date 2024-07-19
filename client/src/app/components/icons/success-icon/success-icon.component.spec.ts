import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SuccessIconComponent } from './success-icon.component';

describe('SuccessIconComponent', () => {
  let component: SuccessIconComponent;
  let fixture: ComponentFixture<SuccessIconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SuccessIconComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SuccessIconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
