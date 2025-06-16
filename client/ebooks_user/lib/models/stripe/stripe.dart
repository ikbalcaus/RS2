import "package:json_annotation/json_annotation.dart";

part "stripe.g.dart";

@JsonSerializable()
class Stripe {
  String? url;

  Stripe({this.url});

  factory Stripe.fromJson(Map<String, dynamic> json) => _$StripeFromJson(json);
  Map<String, dynamic> toJson() => _$StripeToJson(this);
}
