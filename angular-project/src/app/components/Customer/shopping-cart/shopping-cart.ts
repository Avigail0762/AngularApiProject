import { Component, inject } from '@angular/core';
import { CommonModule, AsyncPipe, CurrencyPipe } from '@angular/common';
import { CustomerService } from '../../../services/customer-service';
import { Gift } from '../../../models/giftModel';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-shopping-cart',
  imports: [AsyncPipe, CurrencyPipe, CommonModule],
  templateUrl: './shopping-cart.html',
  styleUrl: './shopping-cart.scss',
})
export class ShoppingCart {
  private customerSrv = inject(CustomerService);

  cart$ = this.customerSrv.getCartForCurrentUser();

  refresh() {
    this.cart$ = this.customerSrv.getCartForCurrentUser();
  }

asItems(data: any): (Gift & { quantity: number })[] {
  const items: Gift[] = Array.isArray(data) ? data : (data?.items ?? []);
  const map = new Map<string, { item: Gift; quantity: number }>();

  items.forEach(gift => {
    if (map.has(gift.name)) {
      map.get(gift.name)!.quantity += 1;
    } else {
      map.set(gift.name, { item: gift, quantity: 1 });
    }
  });

  return Array.from(map.values()).map(entry => ({
    ...entry.item,
    quantity: entry.quantity
  }));
}

  calcTotal(data: any): number {
    const items = this.asItems(data);
    return items.reduce((sum, g) => sum + (Number(g.price) || 0), 0);
  }
  remove(giftId: number) {
    this.customerSrv.removeFromCartForCurrentUser(giftId).subscribe({
      next: () => {
        this.refresh();
        alert('Removed from cart.');
      },
      error: (err) => alert('Failed to remove: ' + (err?.message || 'Unknown error'))
    });
  }

  purchase() {
    this.customerSrv.purchase().subscribe({
      next: () => {
        alert('Purchase successful!');
        this.refresh();
      },
      error: (err) => alert('Purchase failed: ' + (err?.message || 'Unknown error'))
    });
  }

}
