import "package:ebooks_user/models/users/user.dart";
import "package:json_annotation/json_annotation.dart";

part "publisher_follow.g.dart";

@JsonSerializable()
class PublisherFollow {
  int? userId;
  User? publisher;
  DateTime? modifiedAt;

  PublisherFollow({this.userId, this.publisher, this.modifiedAt});

  factory PublisherFollow.fromJson(Map<String, dynamic> json) =>
      _$PublisherFollowFromJson(json);
  Map<String, dynamic> toJson() => _$PublisherFollowToJson(this);
}
