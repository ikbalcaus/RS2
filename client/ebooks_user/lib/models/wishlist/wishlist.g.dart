// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'wishlist.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Wishlist _$WishlistFromJson(Map<String, dynamic> json) => Wishlist(
  userId: (json['userId'] as num?)?.toInt(),
  book: json['book'] == null
      ? null
      : Book.fromJson(json['book'] as Map<String, dynamic>),
  modifiedAt: json['modifiedAt'] == null
      ? null
      : DateTime.parse(json['modifiedAt'] as String),
);

Map<String, dynamic> _$WishlistToJson(Wishlist instance) => <String, dynamic>{
  'userId': instance.userId,
  'book': instance.book,
  'modifiedAt': instance.modifiedAt?.toIso8601String(),
};
