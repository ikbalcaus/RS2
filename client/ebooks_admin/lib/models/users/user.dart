import "package:json_annotation/json_annotation.dart";

part "user.g.dart";

@JsonSerializable()
class User {
  int? userId;
  String? firstName;
  String? lastName;
  String? userName;
  String? email;
  String? filePath;
  String? deletionReason;
  int? publisherVerifiedById;

  User({
    this.userId,
    this.firstName,
    this.lastName,
    this.userName,
    this.email,
    this.filePath,
    this.deletionReason,
    this.publisherVerifiedById,
  });

  factory User.fromJson(Map<String, dynamic> json) => _$UserFromJson(json);
  Map<String, dynamic> toJson() => _$UserToJson(this);
}
