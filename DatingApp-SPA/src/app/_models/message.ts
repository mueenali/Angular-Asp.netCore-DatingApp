export class Message {
  id: number;
  senderId: number;
  senderKnownAs: string;
  senderPhotoUrl: string;
  receiverId: number;
  receiverKnownAs: string;
  receiverPhotoUrl: string;
  content: string;
  isRead: boolean;
  dateRead: Date;
  dateSent: Date;
}
