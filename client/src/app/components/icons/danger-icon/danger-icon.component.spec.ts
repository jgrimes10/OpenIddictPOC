import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DangerIconComponent } from './danger-icon.component';

describe('DangerIconComponent', () => {
  let component: DangerIconComponent;
  let fixture: ComponentFixture<DangerIconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DangerIconComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DangerIconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
