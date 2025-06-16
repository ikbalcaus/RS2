import "package:ebooks_user/models/publisher_follow/publisher_follow.dart";
import "package:ebooks_user/providers/base_provider.dart";

class PublisherFollowsProvider extends BaseProvider<PublisherFollow> {
  PublisherFollowsProvider() : super("publisherfollows");

  @override
  PublisherFollow fromJson(data) {
    return PublisherFollow.fromJson(data);
  }
}
