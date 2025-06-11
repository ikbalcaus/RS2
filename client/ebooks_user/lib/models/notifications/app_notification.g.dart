// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'app_notification.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AppNotification _$AppNotificationFromJson(Map<String, dynamic> json) =>
    AppNotification(
      notificationId: (json['notificationId'] as num?)?.toInt(),
      userId: (json['userId'] as num?)?.toInt(),
      bookId: (json['bookId'] as num?)?.toInt(),
      publisherId: (json['publisherId'] as num?)?.toInt(),
      message: json['message'] as String?,
      isRead: json['isRead'] as bool?,
    );

Map<String, dynamic> _$AppNotificationToJson(AppNotification instance) =>
    <String, dynamic>{
      'notificationId': instance.notificationId,
      'userId': instance.userId,
      'bookId': instance.bookId,
      'publisherId': instance.publisherId,
      'message': instance.message,
      'isRead': instance.isRead,
    };
