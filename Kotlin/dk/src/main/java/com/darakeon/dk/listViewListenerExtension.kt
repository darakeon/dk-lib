package com.darakeon.dk

import android.view.View
import android.widget.AdapterView
import android.widget.ListView

fun ListView.setItemLongClick(
	longClick: (
		parent: AdapterView<*>,
		view: View,
		position: Int,
		id: Long
	) -> Boolean
) {
	onItemLongClickListener = ItemLongClickListener(longClick)
}

fun ListView.setItemLongClick(
	longClick: (
		view: View,
		position: Int
	) -> Boolean
) {
	onItemLongClickListener = ItemLongClickListener(longClick)
}

private class ItemLongClickListener : AdapterView.OnItemLongClickListener {
	private var longClick1:
		((parent: AdapterView<*>, view: View, position: Int, id: Long) -> Boolean)? = null
	constructor(
		longClick: (parent: AdapterView<*>, view: View, position: Int, id: Long) -> Boolean
	) {
		longClick1 = longClick
	}

	private var longClick2:
		((view: View, position: Int) -> Boolean)? = null
	constructor(
		longClick: (view: View, position: Int) -> Boolean
	) {
		longClick2 = longClick
	}

	override fun onItemLongClick(
		parent: AdapterView<*>,
		view: View,
		position: Int,
		id: Long
	): Boolean {
		val fun1 = longClick1
		val fun2 = longClick2

		if (fun1 != null)
			return fun1(parent, view, position, id)
		else if (fun2 != null)
			return fun2(view, position)

		return false
	}
}