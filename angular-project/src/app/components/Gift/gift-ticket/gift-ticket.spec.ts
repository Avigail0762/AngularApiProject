import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GiftTicket } from './gift-ticket';

describe('GiftTicket', () => {
  let component: GiftTicket;
  let fixture: ComponentFixture<GiftTicket>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GiftTicket]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GiftTicket);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
