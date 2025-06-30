import "package:ebooks_user/models/books/book.dart";
import "package:ebooks_user/screens/book_details_screen.dart";
import "package:ebooks_user/screens/publisher_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:flutter/material.dart";

class BookCardView extends StatelessWidget {
  final Book book;
  final Map<String, VoidCallback?>? popupActions;
  const BookCardView({super.key, required this.book, this.popupActions});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: InkWell(
        onTap: () => Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => BookDetailsScreen(bookId: book.bookId!),
          ),
        ),
        child: Material(
          elevation: 1,
          borderRadius: BorderRadius.all(Radius.circular(4)),
          child: SizedBox(
            height: 210,
            child: Row(
              children: [
                ClipRRect(
                  borderRadius: BorderRadius.circular(4),
                  child: Image.network(
                    "${Globals.apiAddress}/images/books/${book.filePath}.webp",
                    width: 140,
                    height: double.infinity,
                    fit: BoxFit.cover,
                    errorBuilder: (context, error, stackTrace) =>
                        const Icon(Icons.broken_image, size: 140),
                  ),
                ),
                Expanded(
                  child: Padding(
                    padding: const EdgeInsets.only(
                      top: 7,
                      right: 7,
                      bottom: 9,
                      left: 10,
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    book.title ?? "",
                                    style: const TextStyle(
                                      fontSize: 19.5,
                                      fontWeight: FontWeight.w500,
                                    ),
                                    overflow: TextOverflow.ellipsis,
                                  ),
                                  const SizedBox(height: 2),
                                  Text(
                                    "${book.numberOfViews} views • ${book.modifiedAt?.day}.${book.modifiedAt?.month}.${book.modifiedAt?.year}",
                                    style: const TextStyle(fontSize: 12),
                                  ),
                                ],
                              ),
                            ),
                            SizedBox(
                              width: 35,
                              height: 35,
                              child: PopupMenuButton<String>(
                                icon: const Icon(Icons.more_vert, size: 17),
                                onSelected: (String title) {
                                  final callback = popupActions?[title];
                                  if (callback != null) {
                                    Future.microtask(() => callback());
                                  }
                                },
                                itemBuilder: (context) {
                                  return popupActions?.entries.map((entry) {
                                        return PopupMenuItem(
                                          value: entry.key,
                                          child: Text(entry.key),
                                        );
                                      }).toList() ??
                                      [];
                                },
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 8),
                        GestureDetector(
                          onTap: () => Navigator.push(
                            context,
                            MaterialPageRoute(
                              builder: (context) => PublisherScreen(
                                publisherId: book.publisher!.userId!,
                              ),
                            ),
                          ),
                          child: Row(
                            children: [
                              ClipOval(
                                child: Image.network(
                                  "${Globals.apiAddress}/images/users/${book.publisher?.filePath}.webp",
                                  height: 18,
                                  width: 18,
                                  fit: BoxFit.cover,
                                  errorBuilder: (context, error, stackTrace) =>
                                      const Icon(
                                        Icons.account_circle,
                                        size: 18,
                                      ),
                                ),
                              ),
                              const SizedBox(width: 6),
                              Text(
                                book.publisher?.userName ?? "",
                                style: const TextStyle(
                                  fontSize: 12,
                                  fontWeight: FontWeight.w600,
                                ),
                                overflow: TextOverflow.ellipsis,
                              ),
                              const SizedBox(width: 4),
                              if (book.publisher?.publisherVerifiedById != null)
                                const Padding(
                                  padding: EdgeInsets.only(top: 2),
                                  child: Icon(
                                    Icons.verified,
                                    color: Colors.green,
                                    size: 12,
                                  ),
                                ),
                            ],
                          ),
                        ),
                        const SizedBox(height: 12),
                        Text(
                          book.description ?? "",
                          style: const TextStyle(fontSize: 11),
                          maxLines: 3,
                          overflow: TextOverflow.ellipsis,
                        ),
                        const Spacer(),
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Text(
                              "${book.price?.toStringAsFixed(2) ?? "0.00"}€",
                              style: const TextStyle(
                                fontWeight: FontWeight.w700,
                                fontSize: 16,
                              ),
                            ),
                            Padding(
                              padding: const EdgeInsets.only(top: 4, right: 8),
                              child: Row(
                                children: List.generate(5, (index) {
                                  double rating = book.averageRating ?? 0;
                                  if (rating >= index + 1) {
                                    return const Icon(Icons.star, size: 12);
                                  } else if (rating >= index + 0.5) {
                                    return const Icon(
                                      Icons.star_half,
                                      size: 12,
                                    );
                                  } else {
                                    return const Icon(
                                      Icons.star_border,
                                      size: 12,
                                    );
                                  }
                                }),
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
