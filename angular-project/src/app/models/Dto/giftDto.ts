export class GiftDTO{
    name!: string;
    price!: number;
    description?: string;
    donorId!: number;
    buyersNumber: number = 0;
    category?: string;
    winnerTicketId?: number; 
    isDrawn: boolean = false;
}