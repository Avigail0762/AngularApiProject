import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LotteryService } from '../../services/lottery-service';
import { GiftsService } from '../../services/gifts-service';

@Component({
  selector: 'app-lottery',
  imports: [FormsModule,CommonModule],
  templateUrl: './lottery.html',
  styleUrl: './lottery.scss',
})
export class Lottery {
  giftsSrv: GiftsService = inject(GiftsService);
  lotterySrv = inject(LotteryService);

  gifts$ = this.giftsSrv.getAll();
  totalIncome: number | null = null;

  draw(giftId: number) {
    this.lotterySrv.draw(giftId).subscribe((data) => {
      console.log('Drawn Ticket:', data);
      this.getAll(); // רענון
    });
  }

  getAll() {
    this.gifts$ = this.giftsSrv.getAll();
  }

  downloadWinnersReport() {
  this.lotterySrv.getWinners().subscribe(winners => {
    if (!winners || winners.length === 0) {
      alert('אין זוכים');
      return;
    }

    let content = 'דו"ח זוכים בהגרלה\n';
    content += '----------------------\n';

    winners.forEach(w => {
      content += `GiftId: ${w.giftId} | UserId: ${w.userId} | TicketId: ${w.id} | Quantity: ${w.quantity}\n`;
    });

    const blob = new Blob([content], { type: 'text/plain;charset=utf-8;' });
    const url = window.URL.createObjectURL(blob);

    const a = document.createElement('a');
    a.href = url;
    a.download = 'winners.txt';
    a.click();

    window.URL.revokeObjectURL(url);
  });
  }

  getTotalIncome() {
  this.lotterySrv.getTotalIncome().subscribe(total => {
    this.totalIncome = total;
  });
  }

}
