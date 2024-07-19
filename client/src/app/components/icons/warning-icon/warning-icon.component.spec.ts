import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WarningIconComponent } from './warning-icon.component';

describe('WarningIconComponent', () => {
  let component: WarningIconComponent;
  let fixture: ComponentFixture<WarningIconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WarningIconComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WarningIconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
