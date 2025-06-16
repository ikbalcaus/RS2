// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'publisher_follow.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublisherFollow _$PublisherFollowFromJson(Map<String, dynamic> json) =>
    PublisherFollow(
      userId: (json['userId'] as num?)?.toInt(),
      publisher: json['publisher'] == null
          ? null
          : User.fromJson(json['publisher'] as Map<String, dynamic>),
      modifiedAt: json['modifiedAt'] == null
          ? null
          : DateTime.parse(json['modifiedAt'] as String),
    );

Map<String, dynamic> _$PublisherFollowToJson(PublisherFollow instance) =>
    <String, dynamic>{
      'userId': instance.userId,
      'publisher': instance.publisher,
      'modifiedAt': instance.modifiedAt?.toIso8601String(),
    };
