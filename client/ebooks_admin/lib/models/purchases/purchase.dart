import "package:ebooks_admin/models/books/book.dart";
import "package:ebooks_admin/models/users/user.dart";
import "package:json_annotation/json_annotation.dart";

part "purchase.g.dart";

@JsonSerializable()
class Purchase {
  int? purchaseId;
  User? user;
  User? publisher;
  Book? book;
  DateTime? createdAt;
  double? totalPrice;
  String? paymentStatus;
  String? paymentMethod;
  String? transactionId;
  String? failureMessage;
  String? failureCode;
  String? failureReason;

  Purchase({
    this.purchaseId,
    this.user,
    this.publisher,
    this.book,
    this.createdAt,
    this.totalPrice,
    this.paymentMethod,
    this.paymentStatus,
    this.transactionId,
    this.failureMessage,
    this.failureCode,
    this.failureReason,
  });

  factory Purchase.fromJson(Map<String, dynamic> json) =>
      _$PurchaseFromJson(json);
  Map<String, dynamic> toJson() => _$PurchaseToJson(this);
}
