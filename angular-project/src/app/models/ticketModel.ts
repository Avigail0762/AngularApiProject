export class Ticket{
  id!: number;
  userId!: number;
  giftId!: number;
  ticketNumberForGift?: number;
  quantity: number=1;
  purchasedAt!: Date;
}