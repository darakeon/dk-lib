Feature: Base64

Scenario: Decode
	Given the text VGl0bGU=
	When ask to decode it
	Then the new text will be Title
