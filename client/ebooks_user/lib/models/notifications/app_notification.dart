import "package:json_annotation/json_annotation.dart";

part "app_notification.g.dart";

@JsonSerializable()
class AppNotification {
  int? notificationId;
  int? userId;
  int? bookId;
  int? publisherId;
  String? message;
  bool? isRead;

  AppNotification({
    this.notificationId,
    this.userId,
    this.bookId,
    this.publisherId,
    this.message,
    this.isRead,
  });

  factory AppNotification.fromJson(Map<String, dynamic> json) =>
      _$AppNotificationFromJson(json);
  Map<String, dynamic> toJson() => _$AppNotificationToJson(this);
}
