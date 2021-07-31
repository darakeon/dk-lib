Feature: Eml Reader

Scenario: 01. Existent file
	Given the file name nami-sense-of-direction.eml
	When file read is called
	Then the result is not null

Scenario: 02. Non existent file
	Given the file name zoro-sense-of-direction.eml
	When file read is called
	Then the result is null

Scenario: 03. Content with creation date
	Given the content Test
		And the creation date 2021-07-29 23:13:54
	When content read is called
	Then the body is
			| Body |
			| Test |
		And the creation date is 2021-07-29 23:13:54

Scenario: 04. Content without creation date
	Given the content Test
	When content read is called
	Then the body is
			| Body |
			| Test |
		And the creation date is null

Scenario: 05. Content with line break symbols inside
	Given the content Test\nLines
	When content read is called
	Then the body is
			| Body  |
			| Test  |
			| Lines |

Scenario: 06. Content split into lines
	Given the content
			| Content |
			| Test    |
			| Lines   |
	When content read is called
	Then the body is
			| Body  |
			| Test  |
			| Lines |

Scenario: 07. Simple headers
	Given the content
			| Content      |
			| Header: Test |
			|              |
			| Content      |
	When content read is called
	Then the body is
			| Body    |
			| Content |
		And the header is
			| Key    | Value |
			| Header | Test  |

Scenario: 08. Multiline headers
	Given the content
			| Content      |
			| Header: Test |
			| & Continue   |
			|              |
			| Content      |
	When content read is called
	Then the body is
			| Body    |
			| Content |
		And the header is
			| Key    | Value         |
			| Header | Test Continue |

Scenario: 09. Multiline headers with tab
	Given the content
			| Content      |
			| Header: Test |
			| &	Continue   |
			|              |
			| Content      |
	When content read is called
	Then the body is
			| Body    |
			| Content |
		And the header is
			| Key    | Value         |
			| Header | Test	Continue |

Scenario: 10. With subject
	Given the content
			| Content        |
			| Subject: Title |
			|                |
			| Content        |
	When content read is called
	Then the body is
			| Body    |
			| Content |
		And the subject is Title
		
Scenario: 11. With encoded subject
	Given the content
			| Content                       |
			| Subject: =?utf-8?B?VGl0bGU=?= |
			|                               |
			| Content                       |
	When content read is called
	Then the body is
			| Body    |
			| Content |
		And the subject is Title

Scenario: 12. Without subject
	Given the content
			| Content |
			| Content |
	When content read is called
	Then the body is
			| Body    |
			| Content |
		And the subject is null

Scenario: 13. With content transfer encoding base64
	Given the content
			| Content                           |
			| Content-Transfer-Encoding: base64 |
			|                                   |
			| Q29udGVudA==                      |
	When content read is called
	Then the body is
			| Body    |
			| Content |
		And the subject is null

Scenario: 14. Without content transfer encoding
	Given the content
			| Content |
			| plain   |
	When content read is called
	Then the body is
			| Body  |
			| plain |
		And the subject is null
