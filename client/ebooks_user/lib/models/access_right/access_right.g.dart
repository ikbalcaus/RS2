// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'access_right.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AccessRight _$AccessRightFromJson(Map<String, dynamic> json) => AccessRight(
  userId: (json['userId'] as num?)?.toInt(),
  book: json['book'] == null
      ? null
      : Book.fromJson(json['book'] as Map<String, dynamic>),
  modifiedAt: json['modifiedAt'] == null
      ? null
      : DateTime.parse(json['modifiedAt'] as String),
  isFavorite: json['isFavorite'] as bool?,
  isHidden: json['isHidden'] as bool?,
);

Map<String, dynamic> _$AccessRightToJson(AccessRight instance) =>
    <String, dynamic>{
      'userId': instance.userId,
      'book': instance.book,
      'modifiedAt': instance.modifiedAt?.toIso8601String(),
      'isFavorite': instance.isFavorite,
      'isHidden': instance.isHidden,
    };
