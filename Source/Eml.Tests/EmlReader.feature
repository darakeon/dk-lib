Feature: A. Eml Reader

Scenario: A01. Existent file
	Given the file name nami-sense-of-direction.eml
	When file read is called
	Then the result is not null

Scenario: A02. Non existent file
	Given the file name zoro-sense-of-direction.eml
	When file read is called
	Then the result is null

Scenario: A03. Content with creation date
	Given the content Test
		And the creation date 2021-07-29 23:13:54
	When content read is called
	Then the body is
			| Body |
			| Test |
		And the creation date is 2021-07-29 23:13:54

Scenario: A04. Content without creation date
	Given the content Test
	When content read is called
	Then the body is
			| Body |
			| Test |
		And the creation date is null

Scenario: A05. Content with line break symbols inside
	Given the content Test\nLines
	When content read is called
	Then the body is
			| Body  |
			| Test  |
			| Lines |

Scenario: A06. Content split into lines
	Given the content
			| Content |
			| Test    |
			| Lines   |
	When content read is called
	Then the body is
			| Body  |
			| Test  |
			| Lines |

Scenario: A07. Simple headers
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

Scenario: A08. Multiline headers
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

Scenario: A09. Multiline headers with tab
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

Scenario: A10. With subject
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
		
Scenario: A11. With encoded subject
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

Scenario: A12. Without subject
	Given the content
			| Content |
			| Content |
	When content read is called
	Then the body is
			| Body    |
			| Content |
		And the subject is null

Scenario: A13. With content transfer encoding base64
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

Scenario: A14. Without content transfer encoding
	Given the content
			| Content |
			| plain   |
	When content read is called
	Then the body is
			| Body  |
			| plain |
		And the subject is null

Scenario: A15. Multiple content types
	Given the content
			| Content                              |
			| Content-Type: multipart/alternative; |
			| &	boundary="----=_BOUND_=----"       |
			|                                      |
			| MULTI                                |
			|                                      |
			| ------=_BOUND_=----                  |
			| Content-Type: text/plain;            |
			| &	charset="utf-8"                    |
			|                                      |
			| JUST TEXT                            |
			|                                      |
			| ------=_BOUND_=----                  |
			| Content-Type: text/html;             |
			| &	charset="utf-8"                    |
			|                                      |
			| <HTML></HTML>                        |
			|                                      |
			| ------=_BOUND_=------                |
	When content read is called
	Then the header is
			| Key          | Value                                      |
			| Content-Type | multipart/alternative text/plain text/html |
		And the body is
			| Body                                                                                                                                                                                                |
			| <div style='text-align: center; font-family: courier new; padding: 3px; background: #000; border-top: 6px double #C00; border-bottom: 6px double #80C; color: #CCC; font-weight: bold;'>PLAIN</div> |
			| <pre>JUST TEXT</pre><br />                                                                                                                                                                          |
			| <br />                                                                                                                                                                                              |
			| <div style='text-align: center; font-family: courier new; padding: 3px; background: #000; border-top: 6px double #C00; border-bottom: 6px double #80C; color: #CCC; font-weight: bold;'>HTML</div>  |
			| <HTML></HTML>                                                                                                                                                                                       |

Scenario: A16. Repeated headers
	Given the content
			| Content    |
			| Header: H1 |
			| Header: H2 |
			|            |
			| Content    |
	When content read is called
	Then the body is
			| Body    |
			| Content |
		And the header is
			| Key    | Value |
			| Header | H1 H2 |

Scenario: A17. Repeated non-sequencial headers
	Given the content
			| Content     |
			| Header1: H1 |
			| Header2: H2 |
			| Header1: H3 |
			|             |
			| Content     |
	When content read is called
	Then the body is
			| Body    |
			| Content |
		And the header is
			| Key     | Value |
			| Header1 | H1 H3 |
			| Header2 | H2    |

Scenario: A18. Repeated non-sequencial multiline headers
	Given the content
			| Content     |
			| Header1: H1 |
			| & H2        |
			| Header2: H3 |
			| Header1: H4 |
			| & H5        |
			|             |
			| Content     |
	When content read is called
	Then the body is
			| Body    |
			| Content |
		And the header is
			| Key     | Value       |
			| Header1 | H1 H2 H4 H5 |
			| Header2 | H3          |
