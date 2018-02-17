package com.darakeon.dk

import android.support.design.widget.NavigationView
import android.view.MenuItem

fun NavigationView.setItemLongClick(
		longClick: (
				item: MenuItem
		) -> Boolean
) {
	setNavigationItemSelectedListener(com.darakeon.dk.ItemSelectedListener(longClick))
}

private class ItemSelectedListener(
	private val itemSelected: (
		item: MenuItem
	) -> Boolean
) : NavigationView.OnNavigationItemSelectedListener {
	override fun onNavigationItemSelected(item: MenuItem): Boolean {
		return itemSelected(item)
	}
}
