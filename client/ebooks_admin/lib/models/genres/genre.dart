import "package:ebooks_admin/models/users/user.dart";
import "package:json_annotation/json_annotation.dart";

part "genre.g.dart";

@JsonSerializable()
class Genre {
  int? genreId;
  String? name;
  User? modifiedBy;

  Genre({this.genreId, this.name, this.modifiedBy});

  factory Genre.fromJson(Map<String, dynamic> json) => _$GenreFromJson(json);
  Map<String, dynamic> toJson() => _$GenreToJson(this);
}
