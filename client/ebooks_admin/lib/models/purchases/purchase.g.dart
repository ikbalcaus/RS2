// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Purchase _$PurchaseFromJson(Map<String, dynamic> json) => Purchase(
  purchaseId: (json['purchaseId'] as num?)?.toInt(),
  user: json['user'] == null
      ? null
      : User.fromJson(json['user'] as Map<String, dynamic>),
  publisher: json['publisher'] == null
      ? null
      : User.fromJson(json['publisher'] as Map<String, dynamic>),
  book: json['book'] == null
      ? null
      : Book.fromJson(json['book'] as Map<String, dynamic>),
  createdAt: json['createdAt'] == null
      ? null
      : DateTime.parse(json['createdAt'] as String),
  totalPrice: (json['totalPrice'] as num?)?.toDouble(),
  paymentMethod: json['paymentMethod'] as String?,
  paymentStatus: json['paymentStatus'] as String?,
  transactionId: json['transactionId'] as String?,
  failureMessage: json['failureMessage'] as String?,
  failureCode: json['failureCode'] as String?,
  failureReason: json['failureReason'] as String?,
);

Map<String, dynamic> _$PurchaseToJson(Purchase instance) => <String, dynamic>{
  'purchaseId': instance.purchaseId,
  'user': instance.user,
  'publisher': instance.publisher,
  'book': instance.book,
  'createdAt': instance.createdAt?.toIso8601String(),
  'totalPrice': instance.totalPrice,
  'paymentStatus': instance.paymentStatus,
  'paymentMethod': instance.paymentMethod,
  'transactionId': instance.transactionId,
  'failureMessage': instance.failureMessage,
  'failureCode': instance.failureCode,
  'failureReason': instance.failureReason,
};
