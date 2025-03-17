USE CSE_23010101006;

-- Insert into MST_QuizWiseQuestions
CREATE PROCEDURE [dbo].[PR_QuizWiseQuestions_Insert]
    @QuizID		INT,
    @QuestionID	INT,
    @UserID		INT
AS
INSERT INTO [dbo].[MST_QuizWiseQuestions] 
(
	[QuizID],
	[QuestionID],
	[UserID]
)
VALUES
(
	@QuizID,
	@QuestionID,
	@UserID
);


-- Update MST_QuizWiseQuestions
CREATE or alter PROCEDURE [dbo].[PR_QuizWiseQuestions_UpdateByPK]
    @QuizWiseQuestionsID	INT,
    @QuizID					INT,
    @QuestionID				INT,
    @UserID					INT
AS
UPDATE [dbo].[MST_QuizWiseQuestions]
	SET [QuizID] = @QuizID,
		[QuestionID] = @QuestionID,
		[UserID] = @UserID,
		[Modified] = GETDATE()
WHERE [dbo].[MST_QuizWiseQuestions].[QuizWiseQuestionsID] = @QuizWiseQuestionsID;


-- Delete from MST_QuizWiseQuestions
CREATE PROCEDURE [dbo].[PR_QuizWiseQuestions_DeleteByPK]
    @QuizWiseQuestionsID INT
AS
DELETE 
FROM [dbo].[MST_QuizWiseQuestions]
WHERE [dbo].[MST_QuizWiseQuestions].[QuizWiseQuestionsID] = @QuizWiseQuestionsID;


-- Select All from MST_QuizWiseQuestions
CREATE PROCEDURE [dbo].[PR_QuizWiseQuestions_SelectAll]
AS
    SELECT [dbo].[MST_QuizWiseQuestions].[QuizWiseQuestionsID],
           [dbo].[MST_QuizWiseQuestions].[QuizID],
		   [dbo].[MST_Quiz].[QuizName],
           [dbo].[MST_QuizWiseQuestions].[QuestionID],
		   [dbo].[MST_Question].[QuestionText],
           [dbo].[MST_QuizWiseQuestions].[UserID],
		   [dbo].[MST_User].[UserName],
           [dbo].[MST_QuizWiseQuestions].[Created],
           [dbo].[MST_QuizWiseQuestions].[Modified]
    FROM [dbo].[MST_QuizWiseQuestions]

	INNER JOIN [dbo].[MST_User]
	ON [dbo].[MST_User].[UserID] = [dbo].[MST_QuizWiseQuestions].[UserID]

	INNER JOIN [dbo].[MST_Quiz]
	ON [dbo].[MST_Quiz].[QuizID] = [dbo].[MST_QuizWiseQuestions].[QuizID]

	INNER JOIN [dbo].[MST_Question]
	ON [dbo].[MST_Question].[QuestionID] = [dbo].[MST_QuizWiseQuestions].[QuestionID]

	ORDER BY [dbo].[MST_QuizWiseQuestions].[QuizWiseQuestionsID],
			 [dbo].[MST_User].[UserName],
			 [dbo].[MST_Quiz].[QuizName],
			 [dbo].[MST_Question].[QuestionID]

-- Select by Primary Key from MST_QuizWiseQuestions
CREATE PROCEDURE [dbo].[PR_QuizWiseQuestions_SelectByPK]
    @QuizWiseQuestionsID INT
AS
    SELECT [dbo].[MST_QuizWiseQuestions].[QuizWiseQuestionsID],
           [dbo].[MST_QuizWiseQuestions].[QuizID],
           [dbo].[MST_QuizWiseQuestions].[QuestionID],
           [dbo].[MST_QuizWiseQuestions].[UserID],
           [dbo].[MST_QuizWiseQuestions].[Created],
           [dbo].[MST_QuizWiseQuestions].[Modified]
    FROM [dbo].[MST_QuizWiseQuestions]
    WHERE [dbo].[MST_QuizWiseQuestions].[QuizWiseQuestionsID] = @QuizWiseQuestionsID;


-- Delete Quiz and Remove Reference of Quiz 
Create or Alter Procedure PR_QuizwiseQuestions_QuizDelete
	@QuizID Int
As
	Delete From MST_QuizWiseQuestions
	Where QuizID = @QuizID

	Exec PR_Quiz_DeleteByPK @QuizID;

--QuizList With Entered Question Numbers
Create or Alter Procedure PR_QuizwiseQuestions_QuizList
as
	SELECT 
		[dbo].[MST_Quiz].[QuizID],
		[dbo].[MST_Quiz].[QuizName],
		COUNT([dbo].[MST_QuizWiseQuestions].[QuestionID]) AS EnteredQuestions,
		[dbo].[MST_User].[UserName]
	FROM [dbo].[MST_QuizWiseQuestions]

	INNER JOIN [dbo].[MST_Quiz] 
	ON [dbo].[MST_QuizWiseQuestions].[QuizID] = [dbo].[MST_Quiz].[QuizID]

	INNER JOIN [dbo].[MST_User] 
	ON [dbo].[MST_Quiz].[UserID] = [dbo].[MST_User].[UserID]

	GROUP BY [dbo].[MST_Quiz].[QuizID],[dbo].[MST_Quiz].[QuizName],[dbo].[MST_User].[UserName]
	ORDER BY [dbo].[MST_Quiz].[QuizName]


-- Question List Based On QuizID
CREATE OR ALTER PROCEDURE PR_QuizwiseQuestions_QuestionList_From_QuizID
	@QuizID INT
AS
	SELECT
		[dbo].[MST_QuizWiseQuestions].[QuizWiseQuestionsID],
		[dbo].[MST_QuizWiseQuestions].[QuestionID],
		[dbo].[MST_Question].[QuestionText],
		[dbo].[MST_Question].[OptionA],
		[dbo].[MST_Question].[OptionB],
		[dbo].[MST_Question].[OptionC],
		[dbo].[MST_Question].[OptionD],
		[dbo].[MST_Question].[CorrectOption],
		[dbo].[MST_Question].[QuestionMarks]
	FROM [dbo].[MST_QuizWiseQuestions]

	INNER JOIN [dbo].[MST_Question]
	ON [dbo].[MST_QuizWiseQuestions].[QuestionID] = [dbo].[MST_Question].[QuestionID]

	INNER JOIN [dbo].[MST_Quiz]
	ON [dbo].[MST_QuizWiseQuestions].[QuizID] = [dbo].[MST_Quiz].[QuizID]

	WHERE [dbo].[MST_QuizWiseQuestions].[QuizID] = @QuizID

	ORDER BY [dbo].[MST_Question].[QuestionID]


--Question IDs By QuizID
CREATE OR ALTER PROCEDURE PR_QuizwiseQuestions_QuestionIDs_From_QuizID
	@QuizID int
AS
	SELECT
		[dbo].[MST_QuizWiseQuestions].[QuestionID]
	FROM [dbo].[MST_QuizWiseQuestions]
	WHERE [dbo].[MST_QuizWiseQuestions].[QuizID] = @QuizID
	

--Add Multiple Questions into a Quiz
CREATE OR ALTER PROCEDURE PR_QuizwiseQuestions_AddMultipleQuestion_Into_Quiz
	@QuizID INT,
    @UserID INT,
    @QuestionIDs VARCHAR(MAX) -- Comma-separated list: '101,102,103'
AS
BEGIN
	BEGIN TRANSACTION;

    BEGIN TRY
        -- Check if @QuestionIDs is empty
        IF @QuestionIDs IS NULL OR LEN(@QuestionIDs) = 0
        BEGIN
            PRINT 'No questions provided. Operation aborted.';
            ROLLBACK;
            RETURN;
        END

        -- Step 1: Remove only unselected questions 
        DELETE FROM MST_QuizWiseQuestions
        WHERE QuizID = @QuizID
        AND QuestionID NOT IN (SELECT value FROM STRING_SPLIT(@QuestionIDs, ','));

        -- Step 2: Insert only new questions that are not already in the quiz
        INSERT INTO MST_QuizWiseQuestions (QuizID, QuestionID, UserID)
        SELECT @QuizID, value, @UserID
        FROM STRING_SPLIT(@QuestionIDs, ',')
        WHERE value NOT IN (
            SELECT QuestionID FROM MST_QuizWiseQuestions WHERE QuizID = @QuizID
        );

        -- Commit the transaction if everything succeeds
        COMMIT;
        PRINT 'Questions successfully added/updated for the quiz.';
    END TRY
    BEGIN CATCH
        -- Rollback if any error occurs
        ROLLBACK;
        PRINT 'Error occurred: ' + ERROR_MESSAGE();
    END CATCH;
END


Select * from MST_QuizWiseQuestions

Select * from MST_Question;
Select * from MST_Quiz;
Select * from MST_User;

EXEC PR_QuizWiseQuestions_Insert 8,18,4;

EXEC PR_QuizwiseQuestions_QuizList

EXEC PR_QuizwiseQuestions_QuestionList_From_QuizID 8;

EXEC PR_QuizwiseQuestions_QuestionIDs_From_QuizID 8;

EXEC PR_QuizwiseQuestions_AddMultipleQuestion_Into_Quiz 8,2,'16,17,18'

CREATE or Alter PROCEDURE [dbo].[PR_QuizWiseQuestion_SelectAll]
AS
    SELECT [dbo].[MST_Question].[QuestionID],
           [dbo].[MST_Question].[QuestionText],
           [dbo].[MST_Question].[QuestionLevelID],
		   [dbo].[MST_QuestionLevel].[QuestionLevel],
           [dbo].[MST_Question].[OptionA],
           [dbo].[MST_Question].[OptionB],
           [dbo].[MST_Question].[OptionC],
           [dbo].[MST_Question].[OptionD],
           [dbo].[MST_Question].[CorrectOption],
           [dbo].[MST_Question].[QuestionMarks],
           [dbo].[MST_Question].[IsActive]
    FROM [dbo].[MST_Question]

	INNER JOIN [dbo].[MST_User]
	ON [dbo].[MST_User].[UserID] = [dbo].[MST_Question].[UserID]

	INNER JOIN [dbo].[MST_QuestionLevel]
	ON [dbo].[MST_QuestionLevel].[QuestionLevelID] = [dbo].[MST_Question].[QuestionLevelID]

	Where [dbo].[MST_Question].[IsActive] = 1

	ORDER BY [dbo].[MST_Question].[QuestionID],
			 [dbo].[MST_QuestionLevel].[QuestionLevel],
			 [dbo].[MST_User].[UserName]