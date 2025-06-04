import "package:ebooks_admin/models/users/user.dart";
import "package:json_annotation/json_annotation.dart";

part "language.g.dart";

@JsonSerializable()
class Language {
  int? languageId;
  String? name;
  User? modifiedBy;

  Language({this.languageId, this.name, this.modifiedBy});

  factory Language.fromJson(Map<String, dynamic> json) =>
      _$LanguageFromJson(json);
  Map<String, dynamic> toJson() => _$LanguageToJson(this);
}
