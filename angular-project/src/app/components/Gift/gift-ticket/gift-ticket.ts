import { Component, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { TicketService } from '../../../services/ticket-service';
import { Ticket } from '../../../models/ticketModel';

@Component({
  selector: 'app-ticket',
  imports: [CommonModule, DatePipe],
  templateUrl: './gift-ticket.html',
  styleUrl: './gift-ticket.scss',
})
export class GiftTicket {
  private route = inject(ActivatedRoute);
  private ticketSrv = inject(TicketService);

  giftId = Number(this.route.snapshot.paramMap.get('id') ?? 0);
  tickets$?: Observable<Ticket[]>;

  constructor() {
    if (this.giftId > 0) {
      this.tickets$ = this.ticketSrv.getTicketsByGiftId(this.giftId);
    }
  }

}
