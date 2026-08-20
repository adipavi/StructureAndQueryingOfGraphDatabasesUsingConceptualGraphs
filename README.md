# Structure and Querying of Graph Databases Using Conceptual Graphs

A **C# Windows Forms application** for visually defining the structure of a graph database and constructing queries using a conceptual-graph-style interface.

The application converts diagrams created through the GUI into **Cypher queries** and executes them against a **Neo4j** graph database.

## Features

* Visual construction of graph database structures
* Visual construction of graph queries
* Node, property, relationship, and constraint representation
* Automatic generation of Cypher queries
* Creation of nodes and relationships in Neo4j
* Querying an existing Neo4j database
* Display of query results in the application
* Undo and clear operations for the graphical editor
* Local Neo4j connection through the official .NET driver

## Technologies

* **C#**
* **.NET 6**
* **Windows Forms**
* **Neo4j**
* **Neo4j.Driver 5.20.0**
* **Cypher Query Language**
* **Visual Studio**

## How It Works

The application contains two main graphical workflows:

### 1. Defining the database structure

The structure editor allows entities, properties, relationships, and structural constraints to be represented graphically.

For example, the included structure describes entities such as:

```text
N:Person
N:Movie
```

with properties including:

```text
P:name
P:born
P:role
P:title
P:released
```

and relationships such as:

```text
R:ACTED_IN
R:DIRECTED
```

The application interprets the visual diagram and generates Cypher statements using operations such as:

```cypher
CREATE (...)
MATCH (...)
MERGE (...)
```

These statements are then executed against Neo4j.

### 2. Building queries

The query editor uses the same visual approach to describe a graph pattern.

The included example represents a query involving:

```text
Person
  |
 name
  |
filter = 'Adrian'
  |
DIRECTED / ACTED_IN
  |
Movie
```

The graphical representation is converted into a Cypher `MATCH` query and executed against the database.

Results are returned and displayed by the application.

## Visual Graph Notation

The application uses different geometric shapes to visually represent the components of a graph database. Together, these shapes form a conceptual representation that can be translated into Neo4j/Cypher.

### Rectangle — Node / Entity

A **rectangle** represents a **node (entity)** in the graph database.

For example:

```text
┌──────────────┐
│    Person    │
└──────────────┘
```

or:

```text
┌──────────────┐
│    Movie     │
└──────────────┘
```

These correspond to Neo4j nodes such as:

```cypher
(:Person)
(:Movie)
```

Nodes represent the main objects or entities stored in the graph.

---

### Circle — Property

A **circle** represents a **property** belonging to a node or relationship.

For example, a `Person` node can have the properties `name` and `born`:

```text
       (name)
          |
     ┌────────┐
     │ Person │
     └────────┘
          |
       (born)
```

This can correspond to a Neo4j node such as:

```cypher
(:Person {
    name: "Adrian",
    born: 2000
})
```

Properties contain information describing an entity or relationship.

---

### Rhombus / Diamond — Relationship

A **rhombus (diamond)** represents a **relationship** between two nodes.

For example:

```text
┌────────┐       ◇ DIRECTED ◇       ┌────────┐
│ Person │ -----------------------> │ Movie  │
└────────┘                          └────────┘
```

This represents the Neo4j graph pattern:

```cypher
(:Person)-[:DIRECTED]->(:Movie)
```

Another example is:

```text
Person ── ◇ ACTED_IN ◇ ──> Movie
```

which corresponds to:

```cypher
(:Person)-[:ACTED_IN]->(:Movie)
```

Relationships describe how two entities are connected.

A relationship can also have its own properties. For example, an `ACTED_IN` relationship could contain a `role` property.

---

### Arrows — Connections and Direction

**Arrows connect the graphical elements and define how they are related.**

Most importantly, arrows between nodes and relationships indicate the **direction of a graph relationship**.

For example:

```text
Person ──> DIRECTED ──> Movie
```

means:

```cypher
(:Person)-[:DIRECTED]->(:Movie)
```

The direction is important because Neo4j relationships are directed.

For example:

```text
Person ──> DIRECTED ──> Movie
```

expresses that a person directed a movie, rather than the movie directing the person.

Arrows are also used by the application to connect nodes with their properties and other components of the conceptual representation. The program analyses these connections to determine how the graphical diagram should be translated into Cypher.

---

## Example

A conceptual graph such as:

```text
             (name)
                |
                v
          ┌──────────┐
          │  Person  │
          └──────────┘
                |
                v
         ◇ DIRECTED ◇
                |
                v
          ┌──────────┐
          │  Movie   │
          └──────────┘
                |
                v
             (title)
```

represents:

* a `Person` node;
* a `name` property belonging to the person;
* a `DIRECTED` relationship;
* a `Movie` node;
* a `title` property belonging to the movie.

Conceptually, this describes:

```text
Person(name) ── DIRECTED ──> Movie(title)
```

and can be translated into a Cypher graph pattern such as:

```cypher
MATCH (p:Person)-[:DIRECTED]->(m:Movie)
RETURN p.name, m.title
```

### Shape Summary

| Shape             | Represents               | Example                     |
| ----------------- | ------------------------ | --------------------------- |
| Rectangle         | Node / Entity            | `Person`, `Movie`           |
| Circle            | Property                 | `name`, `born`, `title`     |
| Rhombus / Diamond | Relationship             | `DIRECTED`, `ACTED_IN`      |
| Arrow             | Connection and direction | `Person → DIRECTED → Movie` |

This visual notation allows a graph database structure or query to be constructed graphically instead of writing the complete Cypher representation manually.


## Diagram Notation

The application uses prefixes to distinguish the different components of a conceptual graph.

| Prefix | Meaning                       | Example       |
| ------ | ----------------------------- | ------------- |
| `N:`   | Node / Entity                 | `N:Person`    |
| `P:`   | Property                      | `P:name`      |
| `R:`   | Relationship                  | `R:ACTED_IN`  |
| `C:`   | Constraint / comparison value | `C:='Adrian'` |

The diagrams also use structural keywords such as:

```text
has
hasOne
hasMore
isOptional
isSource
isTarget
filter
return
out
```

These describe how the different components of the conceptual representation should be interpreted when generating Cypher.

## Example Data Model

One of the structures included with the project represents people and movies.

A simplified version is:

```text
Person
├── name
├── born
├── ACTED_IN ──> Movie
│   └── role
└── DIRECTED ──> Movie

Movie
├── title
└── released
```

This corresponds naturally to a Neo4j model such as:

```text
(:Person)-[:ACTED_IN]->(:Movie)
(:Person)-[:DIRECTED]->(:Movie)
```

with properties stored on nodes and relationships.

## Project Structure

```text
StructureAndQueryingOfGraphDatabasesUsingConceptualGraphs/
│
├── neo4JGUI/
│   ├── neo4JGUI.sln
│   │
│   └── neo4JGUI/
│       ├── Form1.cs
│       ├── Form1.Designer.cs
│       ├── Form2.cs
│       ├── Form2.Designer.cs
│       ├── Form3.cs
│       ├── Form3.Designer.cs
│       ├── Program.cs
│       ├── Prompt.cs
│       └── neo4JGUI.csproj
│
├── query.txt
├── sturcutre.txt
└── .gitattributes
```

### Main files

**`Form1.cs`**

Contains the visual editor and logic for defining a graph database structure. The diagram is interpreted and converted into Cypher statements that create nodes and relationships in Neo4j.

**`Form2.cs`**

Acts as the navigation interface between the structure editor and query editor.

**`Form3.cs`**

Contains the graphical query builder. It converts the visual representation into a Cypher `MATCH` query, executes it through the Neo4j driver, and processes the returned results.

**`sturcutre.txt`**

Contains an example graphical database structure, including its components and their positions.

**`query.txt`**

Contains an example graphical query representation.

## Requirements

To run the project you will need:

* Windows
* Visual Studio 2022 or another environment capable of building .NET 6 Windows Forms applications
* .NET 6 SDK
* Neo4j database
* Neo4j running locally on port `7687`

The project uses:

```xml
<PackageReference Include="Neo4j.Driver" Version="5.20.0" />
```

## Installation

Clone the repository:

```bash
git clone https://github.com/adipavi/StructureAndQueryingOfGraphDatabasesUsingConceptualGraphs.git
```

Enter the project directory:

```bash
cd StructureAndQueryingOfGraphDatabasesUsingConceptualGraphs/neo4JGUI
```

Open:

```text
neo4JGUI.sln
```

in Visual Studio.

Alternatively, restore the dependencies from the command line:

```bash
dotnet restore
```

and build the project with:

```bash
dotnet build
```

## Neo4j Setup

The application expects a Neo4j database running locally.

The source code connects using port:

```text
7687
```

with connection URIs including:

```text
neo4j://localhost:7687
```

and:

```text
bolt://localhost:7687
```

Start your Neo4j database before using the application's database operations.

You will also need to provide valid Neo4j authentication credentials when prompted by the application.

## Running the Application

From Visual Studio:

1. Open `neo4JGUI.sln`.
2. Restore NuGet packages if necessary.
3. Make sure the Neo4j database is running.
4. Build the solution.
5. Run the application.

From the command line:

```bash
cd neo4JGUI/neo4JGUI
dotnet run
```

## Concept

Traditional graph database queries are normally written directly using a query language such as Cypher.

For example:

```cypher
MATCH (p:Person)-[:DIRECTED]->(m:Movie)
RETURN m
```

This project explores another approach: **representing the intended structure or query graphically and translating that representation into Cypher automatically**.

Conceptually:

```text
[Person] ── DIRECTED ──> [Movie]
```

can be interpreted by the program and transformed into the corresponding Neo4j graph pattern.

This allows graph structures and queries to be expressed at a higher conceptual level before being translated into database operations.

## Example Query

The supplied `query.txt` contains a conceptual query involving a `Person`, the person's `name`, a filter value of `Adrian`, relationships including `DIRECTED` and `ACTED_IN`, and a target `Movie`.

The application processes the graphical representation and constructs a Cypher query of the general form:

```cypher
MATCH (p:Person)-[:DIRECTED]->(m:Movie)
RETURN ...
```

The exact generated query depends on the nodes, properties, filters, relationships, and output selected in the graphical editor.

## Purpose

The project demonstrates how **conceptual graphs can be used as an abstraction layer for graph database modelling and querying**.

Instead of requiring the entire database structure or query to be written manually in Cypher, a graphical representation can describe:

* entities;
* properties;
* relationships;
* relationship direction;
* constraints;
* filters;
* returned values.

The program then translates this representation into operations understood by Neo4j.

## Limitations

This project is primarily an experimental/prototype implementation.

Current limitations include:

* Neo4j is expected to run locally.
* The connection port is defined in the source code.
* The graphical interpretation relies heavily on the positioning and connections between diagram elements.
* The query-generation logic supports the conceptual notation implemented by the application rather than arbitrary Cypher.
* The application targets Windows because it uses Windows Forms and `net6.0-windows`.

## Possible Improvements

Future improvements could include:

* configurable Neo4j connection settings;
* support for remote Neo4j databases;
* improved validation of graphical structures;
* more advanced Cypher generation;
* parameterized Cypher queries;
* support for additional relationship patterns;
* improved error handling;
* saving and loading diagrams in a structured format;
* automatic graph layout;
* visualization of query results;
* migration to a newer .NET version;
* automated tests for Cypher generation.

## Author

**Adi Pavi**

GitHub: `@adipavi`

## License

A license is not currently included in the repository.

If you want others to use, modify, or redistribute the project, consider adding an open-source license such as the MIT License.
