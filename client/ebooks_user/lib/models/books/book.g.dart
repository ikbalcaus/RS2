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
  status: json['status'] as String?,
  rejectionReason: json['rejectionReason'] as String?,
  deletionReason: json['deletionReason'] as String?,
  accessRightExist: json['accessRightExist'] as bool?,
  modifiedAt: json['modifiedAt'] == null
      ? null
      : DateTime.parse(json['modifiedAt'] as String),
  discountPercentage: (json['discountPercentage'] as num?)?.toInt(),
  discountStart: json['discountStart'] == null
      ? null
      : DateTime.parse(json['discountStart'] as String),
  discountEnd: json['discountEnd'] == null
      ? null
      : DateTime.parse(json['discountEnd'] as String),
  publisher: json['publisher'] == null
      ? null
      : User.fromJson(json['publisher'] as Map<String, dynamic>),
  language: json['language'] == null
      ? null
      : Language.fromJson(json['language'] as Map<String, dynamic>),
  bookAuthors: (json['bookAuthors'] as List<dynamic>?)
      ?.map((e) => BookAuthor.fromJson(e as Map<String, dynamic>))
      .toList(),
  bookGenres: (json['bookGenres'] as List<dynamic>?)
      ?.map((e) => BookGenre.fromJson(e as Map<String, dynamic>))
      .toList(),
)..averageRating = (json['averageRating'] as num?)?.toDouble();

Map<String, dynamic> _$BookToJson(Book instance) => <String, dynamic>{
  'bookId': instance.bookId,
  'title': instance.title,
  'description': instance.description,
  'filePath': instance.filePath,
  'price': instance.price,
  'numberOfPages': instance.numberOfPages,
  'numberOfViews': instance.numberOfViews,
  'status': instance.status,
  'rejectionReason': instance.rejectionReason,
  'deletionReason': instance.deletionReason,
  'accessRightExist': instance.accessRightExist,
  'modifiedAt': instance.modifiedAt?.toIso8601String(),
  'discountPercentage': instance.discountPercentage,
  'discountStart': instance.discountStart?.toIso8601String(),
  'discountEnd': instance.discountEnd?.toIso8601String(),
  'publisher': instance.publisher,
  'language': instance.language,
  'bookAuthors': instance.bookAuthors,
  'bookGenres': instance.bookGenres,
  'averageRating': instance.averageRating,
};
