import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateGift } from './update-gift';

describe('UpdateGift', () => {
  let component: UpdateGift;
  let fixture: ComponentFixture<UpdateGift>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateGift]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateGift);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
