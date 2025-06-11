import "package:ebooks_user/models/users/user.dart";
import "package:json_annotation/json_annotation.dart";

part "author.g.dart";

@JsonSerializable()
class Author {
  int? authorId;
  String? name;
  User? modifiedBy;

  Author({this.authorId, this.name, this.modifiedBy});

  factory Author.fromJson(Map<String, dynamic> json) => _$AuthorFromJson(json);
  Map<String, dynamic> toJson() => _$AuthorToJson(this);
}
