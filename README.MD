# Match3 Intern Test

+ Video showcase
[![Video Showcase](https://img.youtube.com/vi/mHSOQYPiwGg/0.jpg)](https://www.youtube.com/watch?v=mHSOQYPiwGg)

+ Task 1: Change item sprite in ItemPrefab (See [Commit #9a3990a](https://github.com/phuocn0302/Match3-InternTest/commit/9a3990a015bd1d7c4997ab413832a4dcbd43421f))

+ Task 2 & 3:
  + Modify board fill: ensure the number of identical items divisible by 3  (See [Commit f7884ab](https://github.com/phuocn0302/Match3-InternTest/commit/f7884ab097a2e8fbb66a93458476d06c5aeb0de5))
  + Add BottomBarController to handle add item/remove item, check mate logic on bar, each action Invoke an event to other class (See [Commit #5549c98
    ](https://github.com/phuocn0302/Match3-InternTest/commit/5549c98a845646db22e6bb4a4df7531a2aa5bf22))
  + Board controller (See [Commit #e046425](https://github.com/phuocn0302/Match3-InternTest/commit/e046425b8194bc41eca0cf65929349010acac3a5)):
    + Now detect cell click instead of move item 
    + Add animation on cell hover
    + Add Autoplay coroutine
    + BoardCleared Event (for level condition)
  + Added 2 new level condition (See [Commit #c67d6ff](https://github.com/phuocn0302/Match3-InternTest/commit/c67d6ff3a889b1e1871b82b7a30b28a3c11a93c8))
    + Normal: Fill up bar -> Lose, Clear Board -> Win
    + TimeAttack: Item on bottom bar can be deselect, if clear board before timer end -> Win, Lose otherwise
  + Route UI and GameManager Logic (See [Commit aa33348](https://github.com/phuocn0302/Match3-InternTest/commit/aa3334832796e98897622a6ca719bd262dc64302), [Commit c45f89d](https://github.com/phuocn0302/Match3-InternTest/commit/c45f89dcd9f652cac67c086104b4e82565742e09)) 
