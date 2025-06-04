// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'book.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Book _$BookFromJson(Map<String, dynamic> json) => Book(
  bookId: (json['bookId'] as num?)?.toInt(),
  title: json['title'] as String?,
  description: json['description'] as String?,
  filePath: json['filePath'] as String?,
  price: (json['price'] as num?)?.toDouble(),
  numberOfPages: (json['numberOfPages'] as num?)?.toInt(),
  numberOfViews: (json['numberOfViews'] as num?)?.toInt(),
  stateMachine: json['stateMachine'] as String?,
  rejectionReason: json['rejectionReason'] as String?,
  deletionReason: json['deletionReason'] as String?,
  modifiedAt: json['modifiedAt'] == null
      ? null
      : DateTime.parse(json['modifiedAt'] as String),
  publisherId: (json['publisherId'] as num?)?.toInt(),
  languageId: (json['languageId'] as num?)?.toInt(),
  discountPercentage: (json['discountPercentage'] as num?)?.toInt(),
  discountStart: json['discountStart'] == null
      ? null
      : DateTime.parse(json['discountStart'] as String),
  discountEnd: json['discountEnd'] == null
      ? null
      : DateTime.parse(json['discountEnd'] as String),
);

Map<String, dynamic> _$BookToJson(Book instance) => <String, dynamic>{
  'bookId': instance.bookId,
  'title': instance.title,
  'description': instance.description,
  'filePath': instance.filePath,
  'price': instance.price,
  'numberOfPages': instance.numberOfPages,
  'numberOfViews': instance.numberOfViews,
  'stateMachine': instance.stateMachine,
  'rejectionReason': instance.rejectionReason,
  'deletionReason': instance.deletionReason,
  'modifiedAt': instance.modifiedAt?.toIso8601String(),
  'publisherId': instance.publisherId,
  'languageId': instance.languageId,
  'discountPercentage': instance.discountPercentage,
  'discountStart': instance.discountStart?.toIso8601String(),
  'discountEnd': instance.discountEnd?.toIso8601String(),
};
