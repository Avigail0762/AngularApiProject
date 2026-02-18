import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddGift } from './add-gift';

describe('AddGift', () => {
  let component: AddGift;
  let fixture: ComponentFixture<AddGift>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddGift]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddGift);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
