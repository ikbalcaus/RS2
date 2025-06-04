// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

User _$UserFromJson(Map<String, dynamic> json) => User(
  userId: (json['userId'] as num?)?.toInt(),
  firstName: json['firstName'] as String?,
  lastName: json['lastName'] as String?,
  userName: json['userName'] as String?,
  email: json['email'] as String?,
  deletionReason: json['deletionReason'] as String?,
  publisherVerifiedById: (json['publisherVerifiedById'] as num?)?.toInt(),
);

Map<String, dynamic> _$UserToJson(User instance) => <String, dynamic>{
  'userId': instance.userId,
  'firstName': instance.firstName,
  'lastName': instance.lastName,
  'userName': instance.userName,
  'email': instance.email,
  'deletionReason': instance.deletionReason,
  'publisherVerifiedById': instance.publisherVerifiedById,
};
