import "package:json_annotation/json_annotation.dart";

part "book.g.dart";

@JsonSerializable()
class Book {
  int? bookId;
  String? title;
  String? description;
  String? filePath;
  double? price;
  int? numberOfPages;
  int? numberOfViews;
  String? stateMachine;
  String? rejectionReason;
  String? deletionReason;
  DateTime? modifiedAt;
  int? publisherId;
  //Publisher? publisher;
  int? languageId;
  //Language? language;
  int? discountPercentage;
  DateTime? discountStart;
  DateTime? discountEnd;

  Book({
    this.bookId,
    this.title,
    this.description,
    this.filePath,
    this.price,
    this.numberOfPages,
    this.numberOfViews,
    this.stateMachine,
    this.rejectionReason,
    this.deletionReason,
    this.modifiedAt,
    this.publisherId,
    this.languageId,
    this.discountPercentage,
    this.discountStart,
    this.discountEnd,
  });

  factory Book.fromJson(Map<String, dynamic> json) => _$BookFromJson(json);
  Map<String, dynamic> toJson() => _$BookToJson(this);
}
