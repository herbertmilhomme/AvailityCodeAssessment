/*
T-SQL should be more in-line with Microsoft SQL Server...
CREATE TABLE [CUSTOMER] (
	[CustId] int primary key NOT NULL,
	[FirstName] nvarchar(75) NULL,
	[LastName] nvarchar(75) NULL
);

CREATE TABLE [ORDER] (
	[OrderId] int primary key NOT NULL,
	[CustomerId] int NULL,
	[OrderDate] datetime NOT NULL
);

CREATE TABLE [ORDERLINE] (
	[OrderLineId] int primary key NOT NULL,
	[OrdId] int NOT NULL,
	[ItemName] nvarchar(75) NULL,
	[Cost] money NOT NULL,
	[Quantity] int NOT NULL
);
*/

--Write a SQL query that will produce a reverse-sorted list (alphabetically by name) of customers (first and last names) whose last name begins with the letter ‘S.’
SELECT *
FROM [CUSTOMER]
WHERE UPPER([LastName]) LIKE 'S%' --just in case the table case pattern is not uniform
ORDER BY [FirstName] DESC, [LastName] DESC

--Write a SQL query that will show the total value of all orders each customer has placed in the past six months. Any customer without any orders should show a $0 value.
SELECT o.[CustomerId], COALESCE(SUM(ol.[Cost] * ol.[Quantity]),'$0') as TOTAL
FROM [ORDER] as o
INNER JOIN [ORDERLINE] as ol ON o.[OrderId] = ol.[OrdId]
WHERE o.[OrderDate] > DATEADD(month, -6, GETDATE())
GROUP BY o.[CustomerId]

--Amend the query from the previous question to only show those customers who have a total order value of more than $100 and less than $500 in the past six months.
SELECT o.[CustomerId], COALESCE(SUM(ol.[Cost] * ol.[Quantity]),'$0') as TOTAL
FROM [ORDER] as o
INNER JOIN [ORDERLINE] as ol ON o.[OrderId] = ol.[OrdId]
WHERE o.[OrderDate] > DATEADD(month, -6, GETDATE())
GROUP BY o.[CustomerId]
	HAVING (SUM(ol.[Cost] * ol.[Quantity]) > 100 AND SUM(ol.[Cost] * ol.[Quantity]) < 500)