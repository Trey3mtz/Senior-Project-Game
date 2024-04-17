# Designing Emergent Behavior-prone AI

## Goals:
- We aim for natural-feeling decision-making (not necessarily accurate decisions).
- We seek modularity in design to iterate as needed.
- We aim to reuse, mix, and match different behavioral aspects of AI to create new behavior quickly.

## First Iteration:

### Utility AI:
- We developed a Utility System that selects the best "Action" based on "Considerations."
- Considerations were given modular Response Curves editable in Unity's Editor for reusability.
- Actions were designed to allow modular assignment of multiple Considerations, enabling diverse behaviors.
- The AI's foundation was a modular data container listing possible "Actions" for decision-making.

### Limitations:
- Despite the flexibility in mixing Actions and Considerations, the underlying logic lacked modularity.
- Checking and modifying important AI aspects required extensive coding due to a lack of abstraction/modularity layers.
- Utility AI excelled in individual AI high-level decisions but fell short when applied to abstract concepts like "Creature."

## Second Iteration:

### Solution:
- Behavior Trees were integrated to allow swapping logic nodes within a Tree's structure based on the Creature's needs.
- Utility AI determined abstract behaviors, while Behavior Trees handled specific tasks based on shared motivations.

### New Issues:
- Behavior Trees' self-contained design posed challenges in returning to the Utility AI system after completing a task.
- Initial attempts to stop the tree upon task completion led to AI rigidity and inefficiency in decision-making.

### Solution:
- Formal node placement within Behavior Trees was implemented to guide AI decision-making and task completion.
- Rules were established for node placement, tree exits, and database management to ensure efficient and flexible behavior execution.

## Results:
- The revised workflow reduced unresolved issues and simplified the development of new behaviors.
- Reusable nodes like "Checks" enhanced flexibility across different AI species.